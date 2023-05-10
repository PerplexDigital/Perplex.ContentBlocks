using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;
using System.Linq;

#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
#elif NETFRAMEWORK
using System.Collections.Specialized;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web.PropertyEditors.Validation;
#endif

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValidator : ComplexEditorValidator
    {
        private readonly ContentBlockUtils _utils;
        private readonly ContentBlocksModelValueDeserializer _deserializer;

#if NET6_0_OR_GREATER
        private readonly IShortStringHelper _shortStringHelper;
#endif

        public ContentBlocksValidator(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
#if NET6_0_OR_GREATER
            IPropertyValidationService validationService,
            IShortStringHelper shortStringHelper) : base(validationService)
#elif NETFRAMEWORK
            PropertyEditorCollection propertyEditorCollection,
            IDataTypeService dataTypeService,
            ILocalizedTextService textService) : base(propertyEditorCollection, dataTypeService, textService)
#endif
        {
            _deserializer = deserializer;
            _utils = utils;
#if NET6_0_OR_GREATER
            _shortStringHelper = shortStringHelper;
#endif
        }

        protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object value)
        {
            var modelValue = _deserializer.Deserialize(value?.ToString());
            if (modelValue == null)
            {
                yield break;
            }

            if (modelValue.Header != null)
            {
                yield return GetValidationModel(modelValue.Header);
            }

            if (modelValue.Blocks?.Any() == true)
            {
                foreach (var block in modelValue.Blocks)
                {
                    yield return GetValidationModel(block);
                }
            }

            ElementTypeValidationModel GetValidationModel(ContentBlockModelValue blockValue)
            {
                IDataType dataType = _utils.GetDataType(blockValue.DefinitionId);

                if (dataType == null)
                {
                    return null;
                }

                var validationModel = new ElementTypeValidationModel("", blockValue.Id);

#if NET6_0_OR_GREATER
                var propType = new PropertyType(_shortStringHelper, dataType) { Alias = "content" };
#elif NETFRAMEWORK
                var propType = new PropertyType(dataType) { Alias = "content" };
#endif
                validationModel.AddPropertyTypeValidation(new PropertyTypeValidationModel(propType, blockValue.Content?.ToString()));

                if (blockValue.Variants?.Any() == true)
                {
                    foreach (var variant in blockValue.Variants)
                    {
#if NET6_0_OR_GREATER
                        var variantPropType = new PropertyType(_shortStringHelper, dataType) { Alias = "content_variant_" + variant.Id.ToString("N") };
#elif NETFRAMEWORK
                        var variantPropType = new PropertyType(dataType) { Alias = "content_variant_" + variant.Id.ToString("N") };
#endif
                        validationModel.AddPropertyTypeValidation(new PropertyTypeValidationModel(variantPropType, variant.Content?.ToString()));
                    }
                }

                return validationModel;
            }
        }
    }
}
