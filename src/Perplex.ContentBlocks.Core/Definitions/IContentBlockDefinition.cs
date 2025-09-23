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
    /// Template to use for the block name. To render the value of a block property, use {{propertyAlias}}.
    /// </summary>
    string BlockNameTemplate { get; }

    /// <summary>
    /// The icon of this block that will show up in the backoffice when a block is added to the page.
    /// </summary>
    string Icon { get; }

    /// <summary>
    /// Preview image that will appear in the backoffice UI when selecting blocks
    /// </summary>
    string PreviewImage { get; }

    /// <summary>
    /// Key of the Element Type to use for this Content Block definition.
    /// </summary>
    Guid ElementTypeKey { get; }

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
