using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValueEditor : DataValueEditor
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
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var modelValue = _deserializer.Deserialize(json);
            if (modelValue == null)
            {
                return null;
            }

            JArray fromEditor(ContentBlockModelValue block)
            {
                if (_utils.GetDataType(block.DefinitionId) is IDataType dataType &&
                    dataType.Editor?.GetValueEditor() is IDataValueEditor valueEditor)
                {
                    var propertyData = new ContentPropertyData(block.Content.ToString(), dataType.Configuration);
                    var ncJson = valueEditor.FromEditor(propertyData, null)?.ToString();

                    if (!string.IsNullOrWhiteSpace(ncJson))
                    {
                        return JArray.Parse(ncJson);
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
                return null;
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
                    if (valueEditor.ToEditor(ncProperty, dataTypeService, culture, segment) is List<JObject> ncValue)
                    {
                        return JArray.FromObject(ncValue);
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
    }
}
