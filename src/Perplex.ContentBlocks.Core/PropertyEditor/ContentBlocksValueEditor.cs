using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueEditor : DataValueEditor, IDataValueReference
{
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ContentBlocksModelValueDeserializer _deserializer;
    private readonly ContentBlocksDataResolver _resolver;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IDataTypeConfigurationCache _dataTypeConfigCache;

    public ContentBlocksValueEditor(
        IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper,
        DataEditorAttribute attribute, ContentBlocksValidator validator, ContentBlocksModelValueDeserializer deserializer,
        ContentBlocksDataResolver resolver, PropertyEditorCollection propertyEditors, IDataTypeConfigurationCache dataTypeConfigCache)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
    {
        Validators.Add(validator);
        _jsonSerializer = jsonSerializer;
        _deserializer = deserializer;
        _resolver = resolver;
        _propertyEditors = propertyEditors;
        _dataTypeConfigCache = dataTypeConfigCache;
    }

    public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)
    {
        var json = property.GetValue(culture, segment)?.ToString();
        if (_deserializer.Deserialize(json) is not ContentBlocksModelValue model ||
            _resolver.Resolve(model) is not { } data ||
            data.Count == 0)
        {
            return base.ToEditor(property, culture, segment);
        }

        BlockToEditor(model.Header);

        foreach (var block in model.Blocks ?? [])
        {
            BlockToEditor(block);
        }

        return model;

        void BlockToEditor(ContentBlockModelValue? block)
        {
            if (block is null)
            {
                return;
            }

            if (data.TryGetValue(block.Id, out var blockData))
            {
                block.Content = ToEditor(blockData);
            }

            foreach (var variant in block.Variants ?? [])
            {
                if (!data.TryGetValue(variant.Id, out var variantData))
                {
                    continue;
                }

                variant.Content = ToEditor(variantData);
            }
        }

        JsonNode? ToEditor(BlockItemData data)
        {
            foreach (var prop in data.PropertyValues)
            {
                prop.Value.PropertyType.Variations = ContentVariation.Nothing;
                var tempProp = new Property(prop.Value.PropertyType);
                tempProp.SetValue(prop.Value.Value);

                IDataEditor? propEditor = _propertyEditors[prop.Value.PropertyType.PropertyEditorAlias];
                if (propEditor is null)
                {
                    continue;
                }

                Guid dataTypeKey = prop.Value.PropertyType.DataTypeKey;
                var configuration = _dataTypeConfigCache.GetConfiguration(dataTypeKey);
                var valEditor = propEditor.GetValueEditor(configuration);
                var convValue = valEditor.ToEditor(tempProp);

                data.RawPropertyValues[prop.Key] = convValue;
            }

            return JsonNode.Parse(_jsonSerializer.Serialize(data));
        }
    }

    public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
    {
        var json = editorValue.Value?.ToString();
        if (_deserializer.Deserialize(json) is not ContentBlocksModelValue model ||
            _resolver.Resolve(model) is not { } data ||
            data.Count == 0)
        {
            return base.FromEditor(editorValue, currentValue);
        }

        BlockFromEditor(model.Header);

        foreach (var block in model.Blocks ?? [])
        {
            BlockFromEditor(block);
        }

        return _jsonSerializer.Serialize(model);

        void BlockFromEditor(ContentBlockModelValue? block)
        {
            if (block is null)
            {
                return;
            }

            if (data.TryGetValue(block.Id, out var blockData))
            {
                block.Content = FromEditor(blockData);
            }

            foreach (var variant in block.Variants ?? [])
            {
                if (!data.TryGetValue(variant.Id, out var variantData))
                {
                    continue;
                }

                variant.Content = FromEditor(variantData);
            }
        }

        JsonNode? FromEditor(BlockItemData data)
        {
            foreach (var prop in data.PropertyValues)
            {
                var configuration = _dataTypeConfigCache.GetConfiguration(prop.Value.PropertyType.DataTypeKey);

                IDataEditor? propEditor = _propertyEditors[prop.Value.PropertyType.PropertyEditorAlias];
                if (propEditor is null)
                {
                    continue;
                }

                var contentPropData = new ContentPropertyData(prop.Value.Value, configuration);
                var newValue = propEditor.GetValueEditor().FromEditor(contentPropData, prop.Value.Value);
                data.RawPropertyValues[prop.Key] = newValue;
            }

            return JsonNode.Parse(_jsonSerializer.Serialize(data));
        }
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        // TODO: Implement

        yield break;
    }


}
