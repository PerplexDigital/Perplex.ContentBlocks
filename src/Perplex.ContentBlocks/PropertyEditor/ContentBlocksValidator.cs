using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System;
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
                return valueEditor.Validators.SelectMany(ve => ve
                    .Validate(blockValue.Content, ValueTypes.Json, dataType.Configuration)
                    .Select(vr =>
                    {
                        // Umbraco 8.7 revamped validation and introduced a ComplexEditorValidationResult class
                        // which inherits from the ValidationResult class. This is in itself a great addition.
                        // However, ContentBlocks is compiled with Umbraco 8.1 and does not know about this type so we cannot
                        // do anything with it here.
                        // We could of course recompile ContentBlocks for 8.7+ and handle this type but that would break compatibility
                        // with all older versions so this is not an option.
                        // As a workaround we will handle the ComplexEditorValidationResult class using JToken instead.

                        var validationResult = JToken.FromObject(vr);
                        var isComplex = validationResult.SelectToken("ValidationResults", false) != null;
                        if (isComplex)
                        {
                            // Umbraco 8.7+
                            var parsed = ParseInternal(validationResult, blockValue.Id);
                            string json = parsed.ToString(formatting: Newtonsoft.Json.Formatting.None);
                            return new ValidationResult(json);
                        }
                        else
                        {
                            // < Umbraco 8.7

                            var memberNames = vr.MemberNames.Select(memberName => memberNamePrefix + memberName);
                            var errorMessage = Regex.Replace(vr.ErrorMessage ?? "", @"^Item \d+:?\s*", "");

                            return new ValidationResult(errorMessage, memberNames);
                        }
                    }));
            }
            catch
            {
                // Nested Content validation will throw in some situations,
                // e.g. when a ContentBlock document type has no properties.
                return Enumerable.Empty<ValidationResult>();
            }
        }

        private static JToken ParseInternal(JToken token, Guid contentBlockId)
        {
            var blockId = token.Value<Guid>("BlockId");
            if (blockId != default)
            {
                return ParseBlock(token, contentBlockId);
            }
            else
            {
                var validationResults = token.SelectToken("ValidationResults", false) as JArray;
                return new JArray(validationResults.Select(nest => ParseInternal(nest, contentBlockId)));
            }
        }

        private static JObject ParseBlock(JToken token, Guid contentBlockId)
        {
            var nestedContentKey = token.Value<Guid>("BlockId");
            string elementTypeAlias = token.Value<string>("ElementTypeAlias");
            var validationResults = token.SelectToken("ValidationResults", false) as JArray;

            var block = new JObject
            {
                ["$id"] = $"{contentBlockId}/{nestedContentKey}",
                ["$elementTypeAlias"] = elementTypeAlias,
                ["ModelState"] = new JObject()
            };

            foreach (var validationResult in validationResults)
            {
                ParseProperty(block, validationResult, contentBlockId);
            }

            return block;
        }

        private static JObject ParseProperty(JObject block, JToken token, Guid contentBlockId)
        {
            string propertyTypeAlias = token.Value<string>("PropertyTypeAlias");

            string errorMessage = token.SelectToken("ValidationResults[0].ErrorMessage", false)?.Value<string>();
            var nested = token.SelectToken("ValidationResults[0].ValidationResults", false) as JArray;
            var modelState = block.SelectToken("ModelState");

            if (nested != null)
            {
                // Complex
                block[propertyTypeAlias] = new JArray(nested.Select(item => ParseInternal(item, contentBlockId)));
                modelState[$"_Properties.{propertyTypeAlias}.invariant.null"] = new JArray(new string[] { "" });
            }
            else
            {
                // Simple
                modelState[$"_Properties.{propertyTypeAlias}.invariant.null.value"] = new JArray(new string[] { errorMessage });
            }

            return block;
        }
    }
}
