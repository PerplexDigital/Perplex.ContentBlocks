using Perplex.ContentBlocks.PropertyEditor.Value;
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
    private readonly ContentBlocksValueDeserializer _deserializer;
    private readonly ContentBlocksValueRefiner _refiner;
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IDataTypeConfigurationCache _dataTypeConfigCache;

    public ContentBlocksValueEditor(
        IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper,
        DataEditorAttribute attribute, ContentBlocksValidator validator, ContentBlocksValueDeserializer deserializer,
        ContentBlocksValueRefiner refiner, PropertyEditorCollection propertyEditors, IDataTypeConfigurationCache dataTypeConfigCache)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
    {
        Validators.Add(validator);
        _jsonSerializer = jsonSerializer;
        _deserializer = deserializer;
        _refiner = refiner;
        _propertyEditors = propertyEditors;
        _dataTypeConfigCache = dataTypeConfigCache;
    }

    public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)
    {
        var json = property.GetValue(culture, segment)?.ToString();
        if (_deserializer.Deserialize(json) is not ContentBlocksValue model)
        {
            return base.ToEditor(property, culture, segment);
        }

        _refiner.Refine(model);

        BlockToEditor(model.Header);

        foreach (var block in model.Blocks ?? [])
        {
            BlockToEditor(block);
        }

        return model;

        void BlockToEditor(ContentBlockValue? block)
        {
            if (block is null)
            {
                return;
            }

            if (block.Content is not null)
            {
                block.Content = ToEditor(block.Content);
            }

            foreach (var variant in block.Variants ?? [])
            {
                if (variant.Content is null)
                {
                    continue;
                }

                variant.Content = ToEditor(variant.Content);
            }
        }

        BlockItemData? ToEditor(BlockItemData data)
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

            return data;
        }
    }

    public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
    {
        var json = editorValue.Value?.ToString();
        if (_deserializer.Deserialize(json) is not ContentBlocksValue model)
        {
            return base.FromEditor(editorValue, currentValue);
        }

        _refiner.Refine(model);

        BlockFromEditor(model.Header);

        foreach (var block in model.Blocks ?? [])
        {
            BlockFromEditor(block);
        }

        return _jsonSerializer.Serialize(model);

        void BlockFromEditor(ContentBlockValue? block)
        {
            if (block is null)
            {
                return;
            }

            if (block.Content is not null)
            {
                block.Content = FromEditor(block.Content);
            }

            foreach (var variant in block.Variants ?? [])
            {
                if (variant.Content is null)
                {
                    continue;
                }

                variant.Content = FromEditor(variant.Content);
            }
        }

        BlockItemData? FromEditor(BlockItemData data)
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

            return data;
        }
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        // TODO: Implement

        yield break;
    }
}
