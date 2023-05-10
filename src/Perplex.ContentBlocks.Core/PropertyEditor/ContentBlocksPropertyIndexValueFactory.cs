#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;
using System.Linq;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksPropertyIndexValueFactory : IPropertyIndexValueFactory, IContentBlocksPropertyIndexValueFactory
    {
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly ContentBlockUtils _utils;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly INestedContentPropertyIndexValueFactory _ncIndexValueFactory;

        public ContentBlocksPropertyIndexValueFactory(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            IShortStringHelper shortStringHelper,
            INestedContentPropertyIndexValueFactory ncIndexValueFactory)
        {
            _deserializer = deserializer;
            _utils = utils;
            _shortStringHelper = shortStringHelper;
            _ncIndexValueFactory = ncIndexValueFactory;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<object>>> GetIndexValues(IProperty property, string culture, string segment, bool published)
        {
            var value = property.GetValue(culture, segment, published);
            var modelValue = _deserializer.Deserialize(value?.ToString());
            if (modelValue == null)
            {
                yield break;
            }

            if (modelValue.Header != null)
            {
                foreach (var indexValue in GetBlockIndexValues(modelValue.Header, culture, segment, published))
                {
                    yield return indexValue;
                }
            }

            if (modelValue.Blocks?.Any() == true)
            {
                foreach (var block in modelValue.Blocks)
                {
                    foreach (var indexValue in GetBlockIndexValues(block, culture, segment, published))
                    {
                        yield return indexValue;
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<object>>> GetBlockIndexValues(ContentBlockModelValue blockValue, string culture, string segment, bool published)
        {
            IDataType dataType = _utils.GetDataType(blockValue.DefinitionId);
            if (dataType == null)
            {
                yield break;
            }

            var ncPropType = new PropertyType(_shortStringHelper, dataType) { Alias = "content" };

            if (culture != null) ncPropType.Variations |= ContentVariation.Culture;
            if (segment != null) ncPropType.Variations |= ContentVariation.Segment;

            var ncProperty = new Property(ncPropType);
            ncProperty.SetValue(blockValue.Content?.ToString(), culture, segment);

            foreach (var indexValue in _ncIndexValueFactory.GetIndexValues(ncProperty, culture, segment, published))
            {
                yield return indexValue;
            };
        }
    }
}
#endif
