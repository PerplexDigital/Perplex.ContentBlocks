using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValidator : IValueValidator
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly Lazy<IContentBlockDefinitionRepository> _definitionRepository;

        public ContentBlocksValidator(
            IDataTypeService dataTypeService,
            ContentBlocksModelValueDeserializer deserializer,
            IFactory factory)
        {
            _dataTypeService = dataTypeService;
            _deserializer = deserializer;
            _definitionRepository = new Lazy<IContentBlockDefinitionRepository>(() => factory.GetInstance<IContentBlockDefinitionRepository>());
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

            var repository = _definitionRepository.Value;
            if (repository == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var definition = repository.GetById(blockValue.DefinitionId);
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
                .Validate(blockValue.Content, ValueTypes.Json, dataType.Configuration)
                .Select(vr =>
                {
                    var memberNames = vr.MemberNames.Select(memberName => memberNamePrefix + memberName);
                    var errorMessage = Regex.Replace(vr.ErrorMessage ?? "", @"^Item \d+:?\s*", "");
                    return new ValidationResult(errorMessage, memberNames);
                })
            );
        }
    }
}
