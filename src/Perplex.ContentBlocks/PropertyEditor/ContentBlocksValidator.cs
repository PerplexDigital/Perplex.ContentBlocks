using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using Semver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValidator : IValueValidator
    {
        private readonly ContentBlockUtils _utils;
        private readonly IRuntimeState _runtimeState;
        private readonly ContentBlocksModelValueDeserializer _deserializer;

        public ContentBlocksValidator(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            IRuntimeState runtimeState)
        {
            _deserializer = deserializer;
            _utils = utils;
            _runtimeState = runtimeState;
        }

        public IEnumerable<ValidationResult> Validate(object value, string valueType, object dataTypeConfiguration)
        {
            ContentBlocksModelValue modelValue = _deserializer.Deserialize(value?.ToString());
            if (modelValue == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            Structure structure = (dataTypeConfiguration as ContentBlocksConfiguration)?.Structure
                // No configuration passed in -> assume everything
                ?? Structure.All;

            var blocksToValidate = GetBlocksToValidate(modelValue, structure);

            if (_runtimeState.SemanticVersion >= new SemVersion(8, 7))
            {
                // Umbraco 8.7's new complex validation needs to be handled very differently.
                return ValidateComplex(blocksToValidate);
            }
            else
            {
                return ValidateSimple(blocksToValidate);
            }
        }

        private IEnumerable<ValidationResult> ValidateComplex(IEnumerable<ContentBlockModelValue> blocksToValidate)
        {
            var complexValidationResults = new JArray();

            foreach (var block in blocksToValidate)
            {
                foreach (var validationResult in Validate(block))
                {
                    var jObj = JObject.FromObject(validationResult);
                    if (Parse(jObj, block.Id) is JObject complexValidationResult)
                    {
                        complexValidationResults.Add(complexValidationResult);
                    };
                }
            }

            if (complexValidationResults.Count == 0)
            {
                return Enumerable.Empty<ValidationResult>();
            }
            else
            {
                var errorMessage = complexValidationResults.ToString(Newtonsoft.Json.Formatting.None);
                return new[] { new ValidationResult(errorMessage) };
            }
        }

        private IEnumerable<ValidationResult> ValidateSimple(IEnumerable<ContentBlockModelValue> blocksToValidate)
        {
            return blocksToValidate.SelectMany(block =>
            {
                // We add a prefix to memberNames of fields with errors
                // so we can display it at the correct Content Block
                string memberNamePrefix = $"#content-blocks-id:{block.Id}#";
                return Validate(block).Select(vr =>
                {
                    var memberNames = vr.MemberNames.Select(memberName => memberNamePrefix + memberName);
                    var errorMessage = Regex.Replace(vr.ErrorMessage ?? "", @"^Item \d+:?\s*", "");
                    return new ValidationResult(errorMessage, memberNames);
                });
            });
        }

        private IEnumerable<ContentBlockModelValue> GetBlocksToValidate(ContentBlocksModelValue modelValue, Structure structure)
        {
            if (modelValue.Header?.IsDisabled == false && structure.HasFlag(Structure.Header))
            {
                yield return modelValue.Header;
            }

            if (structure.HasFlag(Structure.Blocks))
            {
                foreach (var block in modelValue.Blocks)
                {
                    if (block?.IsDisabled == false)
                    {
                        yield return block;
                    }
                }
            }
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

            try
            {
                // Validate the value using all validators that have been defined for the datatype
                return valueEditor.Validators.SelectMany(ve => ve.Validate(blockValue.Content, ValueTypes.Json, dataType.Configuration));
            }
            catch
            {
                // Nested Content validation will throw in some situations,
                // e.g. when a ContentBlock document type has no properties.
                return Enumerable.Empty<ValidationResult>();
            }
        }

        private static JObject Parse(JToken token, Guid? contentBlockId)
        {
            if (token["BlockId"] != null)
            {
                return ParseBlock(token, contentBlockId);
            }
            else if (token.SelectToken("ValidationResults[0]", false) is JObject nested)
            {
                return Parse(nested, contentBlockId);
            }
            else
            {
                throw new ArgumentException("Invalid input", nameof(token));
            }
        }

        private static JObject ParseBlock(JToken token, Guid? contentBlockId)
        {
            var nestedContentKey = token.Value<Guid>("BlockId");
            var elementTypeAlias = token.Value<string>("ElementTypeAlias");
            var validationResults = token.SelectToken("ValidationResults", false) as JArray;

            var block = new JObject
            {
                ["$id"] = contentBlockId.HasValue ? $"{contentBlockId}/{nestedContentKey}" : $"{nestedContentKey}",
                ["$elementTypeAlias"] = elementTypeAlias,
                ["ModelState"] = new JObject()
            };

            foreach (var validationResult in validationResults)
            {
                ParseProperty(block, validationResult);
            }

            return block;
        }

        private static JObject ParseProperty(JObject block, JToken token)
        {
            var propertyTypeAlias = token.Value<string>("PropertyTypeAlias");
            var modelState = block.SelectToken("ModelState");

            if (token.SelectToken("ValidationResults[0].ValidationResults", false) is JArray nested)
            {
                // Complex
                block[propertyTypeAlias] = new JArray(nested.Select(obj => Parse(obj, null)));
                modelState[$"_Properties.{propertyTypeAlias}.invariant.null"] = new JArray(new[] { "" });
            }
            else
            {
                // Simple
                var errorMessage = token.SelectToken("ValidationResults[0].ErrorMessage", false)?.Value<string>();
                modelState[$"_Properties.{propertyTypeAlias}.invariant.null.value"] = new JArray(new[] { errorMessage });
            }

            return block;
        }
    }
}
