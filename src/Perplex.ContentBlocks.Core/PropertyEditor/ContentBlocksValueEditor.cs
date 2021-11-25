using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;
using System.Linq;

#if NET5_0
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
#elif NET472
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
#endif


namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValueEditor : DataValueEditor, IDataValueReference
    {
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly ContentBlockUtils _utils;

#if NET5_0
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
#elif NET472
        public ContentBlocksValueEditor(string view, ContentBlocksModelValueDeserializer deserializer, ContentBlockUtils utils, params IValueValidator[] validators) : base(view, validators)
        {
            _deserializer = deserializer;
            _utils = utils;
        }
#endif

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
                if (block?.Content != null &&
                    _utils.GetDataType(block.DefinitionId) is IDataType dataType &&
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

#if NET5_0
        public override object ToEditor(IProperty property, string culture = null, string segment = null)
#elif NET472
        public override object ToEditor(Property property, IDataTypeService dataTypeService, string culture = null, string segment = null)
#endif

        {
            string json = property.GetValue(culture, segment)?.ToString();
            var modelValue = _deserializer.Deserialize(json);
            if (modelValue == null)
            {
#if NET5_0
                return base.ToEditor(property, culture, segment);
#elif NET472
                return base.ToEditor(property, dataTypeService, culture, segment);
#endif
            }

            JArray toEditor(ContentBlockModelValue block)
            {
                if (block?.Content != null &&
                    _utils.GetDataType(block.DefinitionId) is IDataType dataType &&
                    dataType.Editor?.GetValueEditor() is IDataValueEditor valueEditor)
                {
#if NET5_0
                    var ncPropType = new PropertyType(_shortStringHelper, dataType);
#elif NET472
                    var ncPropType = new PropertyType(dataType);
#endif
                    if (culture != null) ncPropType.Variations |= ContentVariation.Culture;
                    if (segment != null) ncPropType.Variations |= ContentVariation.Segment;

                    var ncProperty = new Property(ncPropType);
                    ncProperty.SetValue(block.Content.ToString(), culture, segment);

                    try
                    {
#if NET5_0
                        if (valueEditor.ToEditor(ncProperty, culture, segment) is object ncValue)
#elif NET472
                        if (valueEditor.ToEditor(ncProperty, dataTypeService, culture, segment) is object ncValue)
#endif
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
}
