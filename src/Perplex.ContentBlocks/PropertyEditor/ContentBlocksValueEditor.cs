﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValueEditor : DataValueEditor, IDataValueReference
    {
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly ContentBlockUtils _utils;

        public ContentBlocksValueEditor(string view, ContentBlocksModelValueDeserializer deserializer, ContentBlockUtils utils, params IValueValidator[] validators) : base(view, validators)
        {
            _deserializer = deserializer;
            _utils = utils;
        }

        public override object FromEditor(ContentPropertyData editorValue, object currentValue)
        {
            string json = editorValue.Value?.ToString();
            var modelValue = _deserializer.Deserialize(json);
            if (modelValue == null)
            {
                return base.FromEditor(editorValue, currentValue);
            }

            JArray fromEditor(ContentBlockModelValue block)
            {
                if (_utils.GetDataType(block.DefinitionId) is IDataType dataType &&
                    dataType.Editor?.GetValueEditor() is IDataValueEditor valueEditor)
                {
                    var propertyData = new ContentPropertyData(block.Content.ToString(), dataType.Configuration);

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
                        return block.Content;
                    }
                }

                // Fallback: return the original value
                return block.Content;
            }

            if (modelValue.Header != null)
            {
                modelValue.Header.Content = fromEditor(modelValue.Header);
            }

            if (modelValue.Blocks?.Any() == true)
            {
                foreach (var block in modelValue.Blocks)
                {
                    block.Content = fromEditor(block);
                }
            }

            return JsonConvert.SerializeObject(modelValue, Formatting.None);
        }

        public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null)
        {
            string json = property.GetValue(culture, segment)?.ToString();
            var modelValue = _deserializer.Deserialize(json);
            if (modelValue == null)
            {
                return base.ToEditor(property, dataTypeService, culture, segment);
            }

            JArray toEditor(ContentBlockModelValue block)
            {
                if (_utils.GetDataType(block.DefinitionId) is IDataType dataType &&
                    dataType.Editor?.GetValueEditor() is IDataValueEditor valueEditor)
                {
                    var ncPropType = new PropertyType(dataType);
                    if (culture != null) ncPropType.Variations |= ContentVariation.Culture;
                    if (segment != null) ncPropType.Variations |= ContentVariation.Segment;

                    var ncProperty = new Property(ncPropType);
                    ncProperty.SetValue(block.Content?.ToString(), culture, segment);

                    try
                    {
                        if (valueEditor.ToEditor(ncProperty, dataTypeService, culture, segment) is object ncValue)
                        {
                            return JArray.FromObject(ncValue);
                        };
                    }
                    catch
                    {
                        return block.Content;
                    }
                }

                // Fallback: return the original value
                return block.Content;
            }

            if (modelValue.Header != null)
            {
                modelValue.Header.Content = toEditor(modelValue.Header);
            }

            if (modelValue.Blocks?.Any() == true)
            {
                foreach (var block in modelValue.Blocks)
                {
                    block.Content = toEditor(block);
                }
            }

            return JObject.FromObject(modelValue);
        }

        public IEnumerable<UmbracoEntityReference> GetReferences(object value)
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
                    return valueEditor.GetReferences(model.Content?.ToString());
                }
                return Enumerable.Empty<UmbracoEntityReference>();
            }

            return result;
        }
    }
}
