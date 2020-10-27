using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValidator : IValueValidator
    {
        private readonly ContentBlockUtils _utils;
        private readonly ContentBlocksModelValueDeserializer _deserializer;

        public ContentBlocksValidator(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils)
        {
            _deserializer = deserializer;
            _utils = utils;
        }

        public IEnumerable<ValidationResult> Validate(object value, string valueType, object dataTypeConfiguration)
        {
            ContentBlocksModelValue modelValue = _deserializer.Deserialize(value?.ToString());
            if (modelValue == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var validationResults = new List<ValidationResult>();

            Structure layout = (dataTypeConfiguration as ContentBlocksConfiguration)?.Structure
                // No configuration passed in -> assume everything
                ?? Structure.All;

            if (modelValue.Header?.IsDisabled == false && layout.HasFlag(Structure.Header))
            {
                validationResults.AddRange(Validate(modelValue.Header));
            }

            var blockValidations = modelValue.Blocks
                .Where(block => !block.IsDisabled && layout.HasFlag(Structure.Blocks))
                .SelectMany(Validate);

            validationResults.AddRange(blockValidations);

            return validationResults;
        }

        private IEnumerable<ValidationResult> Validate(ContentBlockModelValue blockValue)
        {
            if (blockValue == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            IDataType dataType = _utils.GetDataType(blockValue.DefinitionId);

            if (dataType == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var valueEditor = dataType.Editor?.GetValueEditor();
            if (valueEditor == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            // We add a prefix to memberNames of fields with errors
            // so we can display it at the correct Content Block
            string memberNamePrefix = $"#content-blocks-id:{blockValue.Id}#";

            // Validate the value using all validators that have been defined for the datatype
            try
            {
                return valueEditor.Validators
                    .SelectMany(ve => ve
                        .Validate(blockValue.Content, ValueTypes.Json, dataType.Configuration)
                        .Select(vr =>
                        {
                            var memberNames = vr.MemberNames.Select(memberName => memberNamePrefix + memberName);
                            var errorMessage = Regex.Replace(vr.ErrorMessage ?? "", @"^Item \d+:?\s*", "");
                            return new ValidationResult(errorMessage, memberNames);
                        })
                    )
                    .ToList();
            }
            catch
            {
                // Nested Content validation will throw in some situations,
                // e.g. when a ContentBlock document type has no properties.
                return Enumerable.Empty<ValidationResult>();
            }
        }
    }
}
