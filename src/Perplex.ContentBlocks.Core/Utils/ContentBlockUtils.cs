using Perplex.ContentBlocks.Definitions;
using System;

#if NET6_0_OR_GREATER
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using static Umbraco.Cms.Core.Constants.PropertyEditors;
#elif NETFRAMEWORK
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using static Umbraco.Core.Constants.PropertyEditors;
#endif

namespace Perplex.ContentBlocks.Utils
{
    /// <summary>
    /// General ContentBlocks utility functions
    /// </summary>
    public class ContentBlockUtils
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly Lazy<IContentBlockDefinitionRepository> _definitionRepository;

        public ContentBlockUtils(IDataTypeService dataTypeService, Lazy<IContentBlockDefinitionRepository> definitionRepository)
        {
            _dataTypeService = dataTypeService;
            _definitionRepository = definitionRepository;
        }

        /// <summary>
        /// Returns the dataType associated with the ContentBlock with the given definitionId.
        /// </summary>
        /// <param name="definitionId">Id of the ContentBlock definition</param>
        /// <returns></returns>
        public IDataType GetDataType(Guid definitionId)
        {
            var definition = _definitionRepository.Value.GetById(definitionId);
            if (definition == null)
            {
                return null;
            }

            return GetDataType(definition);
        }

        /// <summary>
        /// Returns the dataType associated with the given ContentBlock definition
        /// </summary>
        /// <param name="definition">ContentBlock definition</param>
        /// <returns></returns>
        public IDataType GetDataType(IContentBlockDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
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
                return null;
            }

            if (dataType.EditorAlias != Aliases.NestedContent)
            {
                throw new InvalidOperationException($"DataType should be Nested Content, but was '{dataType.EditorAlias}'");
            }

            return dataType;
        }
    }
}
