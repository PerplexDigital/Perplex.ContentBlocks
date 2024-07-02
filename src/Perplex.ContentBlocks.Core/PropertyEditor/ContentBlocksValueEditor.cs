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
    private readonly DataValueReferenceFactoryCollection _referenceFactories;

    public ContentBlocksValueEditor(
        IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper,
        DataEditorAttribute attribute, ContentBlocksValidator validator, ContentBlocksValueDeserializer deserializer,
        ContentBlocksValueRefiner refiner, PropertyEditorCollection propertyEditors, IDataTypeConfigurationCache dataTypeConfigCache,
        DataValueReferenceFactoryCollection referenceFactories)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
    {
        Validators.Add(validator);
        _jsonSerializer = jsonSerializer;
        _deserializer = deserializer;
        _refiner = refiner;
        _propertyEditors = propertyEditors;
        _dataTypeConfigCache = dataTypeConfigCache;
        _referenceFactories = referenceFactories;
    }

    public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)
    {
        var json = property.GetValue(culture, segment)?.ToString();
        if (_deserializer.Deserialize(json) is not ContentBlocksValue model)
        {
            return base.ToEditor(property, culture, segment);
        }

        _refiner.Refine(model);

        ContentBlocksValueIterator.Iterate(model,
            block => ToEditor(block.Content),
            variant => ToEditor(variant.Content));

        return model;

        void ToEditor(BlockItemData? data)
        {
            if (data is null)
            {
                return;
            }

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

        ContentBlocksValueIterator.Iterate(model,
            block => FromEditor(block.Content),
            variant => FromEditor(variant.Content));

        return _jsonSerializer.Serialize(model);

        void FromEditor(BlockItemData? data)
        {
            if (data is null)
            {
                return;
            }

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
        }
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        if (_deserializer.Deserialize(value?.ToString()) is not ContentBlocksValue model)
        {
            return [];
        }

        _refiner.Refine(model);

        return ContentBlocksValueIterator.Iterate(model,
            block => GetReferences(block.Content),
            variant => GetReferences(variant.Content)
        ).SelectMany(r => r).ToArray();

        UmbracoEntityReference[] GetReferences(BlockItemData? data)
        {
            if (data is null)
            {
                return [];
            }

            var references = new HashSet<UmbracoEntityReference>();

            foreach (var property in data.PropertyValues)
            {
                if (!_propertyEditors.TryGet(property.Value.PropertyType.PropertyEditorAlias, out IDataEditor? dataEditor))
                {
                    continue;
                }

                foreach (var reference in _referenceFactories.GetReferences(dataEditor, property.Value.Value))
                {
                    references.Add(reference);
                }
            }

            return [.. references];
        };
    }
}
