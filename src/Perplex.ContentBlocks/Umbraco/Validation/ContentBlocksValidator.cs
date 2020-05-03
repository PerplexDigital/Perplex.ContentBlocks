using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Umbraco.ModelValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.Umbraco.Validation
{
    public class ContentBlocksValidator : IValueValidator
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentBlockDefinitionRepository _contentBlockDefinitionRepository;
        private readonly ContentBlocksModelValueDeserializer _deserializer;

        public ContentBlocksValidator(
            IDataTypeService dataTypeService,
            IContentBlockDefinitionRepository contentBlockDefinitionRepository,
            ContentBlocksModelValueDeserializer deserializer,
            object configuration = null)
        {
            _dataTypeService = dataTypeService;
            _contentBlockDefinitionRepository = contentBlockDefinitionRepository;
            _deserializer = deserializer;
            Configuration = configuration;
        }

        public object Configuration { get; }

        public IEnumerable<ValidationResult> Validate(object value, string valueType, object dataTypeConfiguration)
        {
            ContentBlocksModelValue modelValue = _deserializer.Deserialize(value?.ToString());
            if (modelValue == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var validationResults = new List<ValidationResult>();

            if (modelValue.Header?.IsDisabled == false)
            {
                validationResults.AddRange(Validate(modelValue.Header));
            }

            validationResults.AddRange(modelValue.Blocks.Where(block => !block.IsDisabled).SelectMany(Validate));

            return validationResults;
        }

        private IEnumerable<ValidationResult> Validate(ContentBlockModelValue blockValue)
        {
            if (blockValue == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var definition = _contentBlockDefinitionRepository.GetById(blockValue.DefinitionId);
            if (definition == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            IDataType dataType = null;
            if (definition.DataTypeId is int dataTypeId)
            {
                dataType = _dataTypeService.GetDataType(dataTypeId);
            }
            else if (definition.DataTypeKey is Guid dataTypeKey)
            {
                dataType = _dataTypeService.GetDataType(dataTypeKey);
            }

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
            return valueEditor.Validators.SelectMany(ve => ve
                .Validate(blockValue.Content, "JSON", dataType.Configuration)
                .Select(vr =>
                {
                    var memberNames = vr.MemberNames.Select(memberName => memberNamePrefix + memberName);
                    var errorMessage = Regex.Replace(vr.ErrorMessage ?? "", @"^Item \d+\s*", "");
                    return new ValidationResult(errorMessage, memberNames);
                })
            );
        }
    }
}
