﻿using Perplex.ContentBlocks.Definitions;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using static Umbraco.Cms.Core.Constants.PropertyEditors;

namespace Perplex.ContentBlocks.Utils;

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
    public IDataType? GetDataType(Guid definitionId)
    {
        var definition = _definitionRepository.Value.GetById(definitionId);
        if (definition is null)
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
    public IDataType? GetDataType(IContentBlockDefinition definition)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        IDataType? dataType = null;

#pragma warning disable CS0618 // DataTypeId will still be used until removed in a next major
        if (definition.DataTypeId is int dataTypeId)
        {
            dataType = _dataTypeService.GetDataType(dataTypeId);
        }
#pragma warning restore CS0618 // DataTypeId will still be used until removed in a next major
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
