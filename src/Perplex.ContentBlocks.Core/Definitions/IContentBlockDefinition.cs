using Umbraco.Cms.Core.Models;

namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockDefinition
{
    /// <summary>
    /// Unique id of this Content Block definition
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Name of this Content Block definition
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of this Content Block definition
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Preview image that will appear in the backoffice UI when selecting blocks
    /// </summary>
    string PreviewImage { get; }

    /// <summary>
    /// Data type id of the Nested Content data type used for this Content Block definition.
    /// Provide either DataTypeId OR DataTypeKey, not both. Leave one of them NULL.
    /// </summary>
    [Obsolete("Use " + nameof(DataTypeKey) + " instead. This will be removed in a next major release.")]
    int? DataTypeId { get; }

    /// <summary>
    /// Data type key of the Nested Content data type used for this Content Block definition.
    /// Provide either DataTypeId OR DataTypeKey, not both. Leave one of them NULL.
    /// </summary>
    Guid? DataTypeKey { get; }

    /// <summary>
    /// Category ids this definition belongs to.
    /// </summary>
    IEnumerable<Guid> CategoryIds { get; }

    /// <summary>
    /// Layouts this block defines. Make sure to specify at least one layout.
    /// </summary>
    IEnumerable<IContentBlockLayout> Layouts { get; }

    /// <summary>
    /// Limits this Content Block definition to only the given document types.
    /// Specify the document type alias (<see cref="IContentTypeBase.Alias"/>).
    /// When configured, this Content Block definition will only show up in the Backoffice UI
    /// on the specified document types.
    /// </summary>
    IEnumerable<string> LimitToDocumentTypes { get; }

    /// <summary>
    /// Limits this Content Block definition to only the given cultures.
    /// When configured, this Content Block definition will only show up in the Backoffice UI
    /// on when editing a specific culture.
    /// </summary>
    IEnumerable<string> LimitToCultures { get; }
}
