﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueEditor : DataValueEditor, IDataValueReference
{
    private readonly ContentBlocksModelValueDeserializer _deserializer;
    private readonly ContentBlockUtils _utils;

    private readonly IShortStringHelper _shortStringHelper;

    public ContentBlocksValueEditor(
        ContentBlocksModelValueDeserializer deserializer,
        ContentBlockUtils utils,
        ILocalizedTextService localizedTextService,
        IShortStringHelper shortStringHelper,
        IJsonSerializer jsonSerializer) : base(localizedTextService, shortStringHelper, jsonSerializer)
    {
        _deserializer = deserializer;
        _utils = utils;
        _shortStringHelper = shortStringHelper;
    }

    public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
    {
        var json = editorValue.Value?.ToString();
        var modelValue = _deserializer.Deserialize(json);
        if (modelValue == null)
        {
            return base.FromEditor(editorValue, currentValue);
        }

        if (modelValue.Header is ContentBlockModelValue header)
        {
            header.Content = FromEditor(header.Content, header.DefinitionId);

            foreach (var variant in header.Variants ?? Enumerable.Empty<ContentBlockVariantModelValue>())
            {
                variant.Content = FromEditor(variant.Content, header.DefinitionId);
            }
        }

        foreach (var block in modelValue.Blocks ?? Enumerable.Empty<ContentBlockModelValue>())
        {
            block.Content = FromEditor(block.Content, block.DefinitionId);

            foreach (var variant in block.Variants ?? Enumerable.Empty<ContentBlockVariantModelValue>())
            {
                variant.Content = FromEditor(variant.Content, block.DefinitionId);
            }
        }

        return JsonConvert.SerializeObject(modelValue, Formatting.None);

        JArray? FromEditor(JArray? blockContent, Guid blockDefinitionId)
        {
            if (blockContent?.ToString() is string content &&
                !string.IsNullOrWhiteSpace(content) &&
                _utils.GetDataType(blockDefinitionId) is IDataType dataType &&
                dataType.Editor?.GetValueEditor() is IDataValueEditor valueEditor)
            {
                var propertyData = new ContentPropertyData(content, dataType.Configuration);

                try
                {
                    var ncJson = valueEditor.FromEditor(propertyData, null)?.ToString();

                    if (!string.IsNullOrWhiteSpace(ncJson))
                    {
                        return JArray.Parse(ncJson);
                    }
                }
                catch
                {
                    return blockContent;
                }
            }

            // Fallback: return the original value
            return blockContent;
        }
    }

    public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)

    {
        var json = property.GetValue(culture, segment)?.ToString();
        var modelValue = _deserializer.Deserialize(json);
        if (modelValue == null)
        {
            return base.ToEditor(property, culture, segment);
        }

        JArray? ToEditor(JArray? blockContent, Guid blockDefinitionId)
        {
            if (blockContent?.ToString() is string content &&
                !string.IsNullOrWhiteSpace(content) &&
                _utils.GetDataType(blockDefinitionId) is IDataType dataType &&
                dataType.Editor?.GetValueEditor() is IDataValueEditor valueEditor)
            {
                var ncPropType = new PropertyType(_shortStringHelper, dataType);
                if (culture != null) ncPropType.Variations |= ContentVariation.Culture;
                if (segment != null) ncPropType.Variations |= ContentVariation.Segment;

                var ncProperty = new Property(ncPropType);
                ncProperty.SetValue(content, culture, segment);

                try
                {
                    if (valueEditor.ToEditor(ncProperty, culture, segment) is object ncValue)
                    {
                        return JArray.FromObject(ncValue);
                    };
                }
                catch
                {
                    return blockContent;
                }
            }

            // Fallback: return the original value
            return blockContent;
        }

        if (modelValue.Header is ContentBlockModelValue header)
        {
            header.Content = ToEditor(header.Content, header.DefinitionId);

            foreach (var variant in header.Variants ?? Enumerable.Empty<ContentBlockVariantModelValue>())
            {
                variant.Content = ToEditor(variant.Content, header.DefinitionId);
            }
        }

        foreach (var block in modelValue.Blocks ?? Enumerable.Empty<ContentBlockModelValue>())
        {
            block.Content = ToEditor(block.Content, block.DefinitionId);

            foreach (var variant in block.Variants ?? Enumerable.Empty<ContentBlockVariantModelValue>())
            {
                variant.Content = ToEditor(variant.Content, block.DefinitionId);
            }
        }

        return JObject.FromObject(modelValue);
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        var result = new List<UmbracoEntityReference>();
        var json = value?.ToString();

        var modelValue = _deserializer.Deserialize(json);
        if (modelValue is null)
            return result;

        if (modelValue.Header != null)
        {
            result.AddRange(GetReferencesByBlock(modelValue.Header));
        }

        if (modelValue.Blocks?.Any() is true)
        {
            foreach (var block in modelValue.Blocks)
            {
                result.AddRange(GetReferencesByBlock(block));
            }
        }

        IEnumerable<UmbracoEntityReference> GetReferencesByBlock(ContentBlockModelValue model)
        {
            if (_utils.GetDataType(model.DefinitionId) is IDataType dataType && dataType.Editor?.GetValueEditor() is IDataValueReference valueEditor)
            {
                var blockReferences = valueEditor.GetReferences(model.Content?.ToString());
                var variantReferences = model.Variants?.SelectMany(v => valueEditor.GetReferences(v.Content?.ToString())) ?? Enumerable.Empty<UmbracoEntityReference>();
                return blockReferences.Concat(variantReferences);
            }
            else
            {
                return Enumerable.Empty<UmbracoEntityReference>();
            }
        }

        return result;
    }
}
