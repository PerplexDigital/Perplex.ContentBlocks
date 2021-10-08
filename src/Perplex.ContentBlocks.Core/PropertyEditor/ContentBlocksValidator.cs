using Newtonsoft.Json.Linq;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

#if NET5_0
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
#elif NET472
using System.Collections.Specialized;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
#endif


namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValidator : IValueValidator
    {
        private readonly ContentBlockUtils _utils;
        private readonly IRuntimeState _runtimeState;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ContentBlocksModelValueDeserializer _deserializer;

        public ContentBlocksValidator(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            IRuntimeState runtimeState,
            IHttpContextAccessor httpContextAccessor)
        {
            _deserializer = deserializer;
            _utils = utils;
            _runtimeState = runtimeState;
            _httpContextAccessor = httpContextAccessor;
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

            if (_runtimeState.SemanticVersion >= "8.7.0")
            {
                // Umbraco 8.7's new complex validation needs to be handled very differently.
                return ValidateComplex(blocksToValidate);
            }
            else
            {
                return ValidateSimple(blocksToValidate);
            }
        }

        private (string culture, string segment) GetCultureAndSegment()
        {
            // "invariant" / "null" is how Umbraco treats null/null in their front-end.
            string culture = "invariant";
            string segment = "null";

#if NET5_0
            if (_httpContextAccessor.HttpContext is HttpContext httpCtx &&
                httpCtx?.Request?.Headers is IHeaderDictionary headers)
            {
                if (headers.TryGetValue("X-UMB-CULTURE", out var cultureValue) && cultureValue.Any())
                {
                    culture = cultureValue.First();
                }

                if (headers.TryGetValue("X-UMB-SEGMENT", out var segmentValue) && segmentValue.Any())
                {
                    segment = segmentValue.First();
                }
            };
#elif NET472
            if (_httpContextAccessor.HttpContext is HttpContext httpCtx &&
                        httpCtx?.Request?.Headers is NameValueCollection headers)
            {
                if (headers["X-UMB-CULTURE"] is string cv && !string.IsNullOrEmpty(cv))
                {
                    culture = cv;
                }

                if (headers["X-UMB-SEGMENT"] is string sv && !string.IsNullOrEmpty(sv))
                {
                    segment = sv;
                }
            };
#endif
            return (culture, segment);
        }

        private IEnumerable<ValidationResult> ValidateComplex(IEnumerable<ContentBlockModelValue> blocksToValidate)
        {
            (string culture, string segment) = GetCultureAndSegment();

            var complexValidationResults = new JArray();

            foreach (var block in blocksToValidate)
            {
                foreach (var (variantId, validationResults) in Validate(block))
                {
                    foreach (var validationResult in validationResults)
                    {
                        var jObj = JObject.FromObject(validationResult);
                        if (Parse(jObj, block.Id, variantId, culture, segment) is JObject complexValidationResult)
                        {
                            complexValidationResults.Add(complexValidationResult);
                        };
                    }
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
                return Validate(block).SelectMany(tup =>
                {
                    var (variantId, validationResults) = tup;

                    // We add a prefix to memberNames of fields with errors
                    // so we can display it at the correct Content Block
                    string memberNamePrefix = $"#content-blocks-id:{block.Id}{(variantId.HasValue ? "/" + variantId : "")}#";

                    return validationResults.Select(vr =>
                    {
                        var memberNames = vr.MemberNames.Select(memberName => memberNamePrefix + memberName);
                        var errorMessage = Regex.Replace(vr.ErrorMessage ?? "", @"^Item \d+:?\s*", "");
                        return new ValidationResult(errorMessage, memberNames);
                    });
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

        private IEnumerable<(Guid? variantId, IEnumerable<ValidationResult> validationResults)> Validate(ContentBlockModelValue blockValue)
        {
            var validationResults = new List<(Guid?, IEnumerable<ValidationResult>)>();

            if (blockValue == null)
            {
                return validationResults;
            }

            IDataType dataType = _utils.GetDataType(blockValue.DefinitionId);

            if (dataType == null)
            {
                return validationResults;
            }

            var valueEditor = dataType.Editor?.GetValueEditor();
            if (valueEditor == null)
            {
                return validationResults;
            }

            try
            {
                // Validate the value using all validators that have been defined for the datatype

                // Block
                validationResults.Add((null, ValidateContent(blockValue.Content)));

                // Variants
                if (blockValue.Variants != null)
                {
                    foreach (var variant in blockValue.Variants)
                    {
                        validationResults.Add((variant.Id, ValidateContent(variant.Content)));
                    }
                }
            }
            catch
            {
                // Nested Content validation will throw in some situations,
                // e.g. when a ContentBlock document type has no properties.                
            }

            return validationResults;

            IEnumerable<ValidationResult> ValidateContent(JArray content)
            {
                try
                {
                    // Validate the value using all validators that have been defined for the datatype
                    return valueEditor.Validators.SelectMany(ve => ve.Validate(content, ValueTypes.Json, dataType.Configuration));
                }
                catch
                {
                    // Nested Content validation will throw in some situations,
                    // e.g. when a ContentBlock document type has no properties.                
                    return Enumerable.Empty<ValidationResult>();
                }
            }
        }

        private static JObject Parse(JToken token, Guid? contentBlockId, Guid? variantId, string culture, string segment)
        {
            if (token["BlockId"] != null)
            {
                return ParseBlock(token, contentBlockId, variantId, culture, segment);
            }
            else if (token.SelectToken("ValidationResults[0]", false) is JObject nested)
            {
                return Parse(nested, contentBlockId, variantId, culture, segment);
            }
            else
            {
                throw new ArgumentException("Invalid input", nameof(token));
            }
        }

        private static JObject ParseBlock(JToken token, Guid? contentBlockId, Guid? variantId, string culture, string segment)
        {
            var nestedContentKey = token.Value<Guid>("BlockId");
            var elementTypeAlias = token.Value<string>("ElementTypeAlias");
            var validationResults = token.SelectToken("ValidationResults", false) as JArray;

            var block = new JObject
            {
                ["$id"] = contentBlockId.HasValue
                    ? variantId.HasValue
                        ? $"{contentBlockId}/{variantId}/{nestedContentKey}"
                        : $"{contentBlockId}/{nestedContentKey}"
                    : $"{nestedContentKey}",
                ["$elementTypeAlias"] = elementTypeAlias,
                ["ModelState"] = new JObject()
            };

            foreach (var validationResult in validationResults)
            {
                ParseProperty(block, validationResult, culture, segment);
            }

            return block;
        }

        private static JObject ParseProperty(JObject block, JToken token, string culture, string segment)
        {
            var propertyTypeAlias = token.Value<string>("PropertyTypeAlias");
            var modelState = block.SelectToken("ModelState");

            if (token.SelectToken("ValidationResults[0].ValidationResults", false) is JArray nested)
            {
                // Complex
                block[propertyTypeAlias] = new JArray(nested.Select(obj => Parse(obj, null, null, culture, segment)));
                modelState[$"_Properties.{propertyTypeAlias}.{culture}.{segment}"] = new JArray(new[] { "" });
            }
            else
            {
                // Simple
                var errorMessage = token.SelectToken("ValidationResults[0].ErrorMessage", false)?.Value<string>();
                modelState[$"_Properties.{propertyTypeAlias}.{culture}.{segment}.value"] = new JArray(new[] { errorMessage });
            }

            return block;
        }
    }
}
