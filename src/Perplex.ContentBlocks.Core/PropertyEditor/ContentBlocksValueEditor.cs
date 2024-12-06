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
    private readonly PropertyEditorCollection _propertyEditors;
    private readonly IDataTypeConfigurationCache _dataTypeConfigCache;
    private readonly DataValueReferenceFactoryCollection _referenceFactories;

    public ContentBlocksValueEditor(
        IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper,
        DataEditorAttribute attribute, ContentBlocksValidator validator, ContentBlocksValueDeserializer deserializer,
        PropertyEditorCollection propertyEditors, IDataTypeConfigurationCache dataTypeConfigCache,
        DataValueReferenceFactoryCollection referenceFactories)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
    {
        Validators.Add(validator);
        _jsonSerializer = jsonSerializer;
        _deserializer = deserializer;
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

        ToEditor(model.Header?.Content);

        if (model.Blocks is not null)
        {
            foreach (var block in model.Blocks)
            {
                ToEditor(block.Content);
            }
        }

        return model;

        void ToEditor(BlockItemData? data)
        {
            if (data is null)
            {
                return;
            }

            foreach (var prop in data.Values)
            {
                if (prop.PropertyType is null)
                {
                    continue;
                }

                prop.PropertyType.Variations = ContentVariation.Nothing;
                var tempProp = new Property(prop.PropertyType);
                tempProp.SetValue(prop.Value);

                IDataEditor? propEditor = _propertyEditors[prop.PropertyType.PropertyEditorAlias];
                if (propEditor is null)
                {
                    continue;
                }

                Guid dataTypeKey = prop.PropertyType.DataTypeKey;
                var configuration = _dataTypeConfigCache.GetConfiguration(dataTypeKey);
                var valEditor = propEditor.GetValueEditor(configuration);
                var convValue = valEditor.ToEditor(tempProp);

                // Update raw value
                prop.Value = convValue;
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

        FromEditor(model.Header?.Content);

        if (model.Blocks is not null)
        {
            foreach (var block in model.Blocks)
            {
                FromEditor(block.Content);
            }
        }

        return _jsonSerializer.Serialize(model);

        void FromEditor(BlockItemData? data)
        {
            if (data is null)
            {
                return;
            }

            foreach (var prop in data.Values)
            {
                if (prop.PropertyType is null)
                {
                    continue;
                }

                var configuration = _dataTypeConfigCache.GetConfiguration(prop.PropertyType.DataTypeKey);

                IDataEditor? propEditor = _propertyEditors[prop.PropertyType.PropertyEditorAlias];
                if (propEditor is null)
                {
                    continue;
                }

                var contentPropData = new ContentPropertyData(prop.Value, configuration);
                var newValue = propEditor.GetValueEditor().FromEditor(contentPropData, prop.Value);

                // Update raw value
                prop.Value = newValue;
            }
        }
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        if (_deserializer.Deserialize(value?.ToString()) is not ContentBlocksValue model)
        {
            yield break;
        }

        if (model.Header is not null)
        {
            foreach (var reference in GetReferences(model.Header.Content))
            {
                yield return reference;
            }
        }

        if (model.Blocks is not null)
        {
            foreach (var block in model.Blocks)
            {
                foreach (var reference in GetReferences(block.Content))
                {
                    yield return reference;
                }
            }
        }

        UmbracoEntityReference[] GetReferences(BlockItemData? data)
        {
            if (data is null)
            {
                return [];
            }

            var references = new HashSet<UmbracoEntityReference>();

            foreach (var property in data.Values)
            {
                if (property.PropertyType is null)
                {
                    continue;
                }

                if (!_propertyEditors.TryGet(property.PropertyType.PropertyEditorAlias, out IDataEditor? dataEditor))
                {
                    continue;
                }

                foreach (var reference in _referenceFactories.GetReferences(dataEditor, property.Value))
                {
                    references.Add(reference);
                }
            }

            return [.. references];
        };
    }
}
