using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockDefinition
{
    Guid Id { get; }
    string Name { get; }
    string Description { get; }
    string PreviewImage { get; }

    /// <summary>
    /// Data type id of the Nested Content data type used for this Content Block definition.
    /// Provide either DataTypeId OR DataTypeKey, not both. Leave one of them NULL.
    /// </summary>
    int? DataTypeId { get; }

    /// <summary>
    /// Data type key of the Nested Content data type used for this Content Block definition.
    /// Provide either DataTypeId OR DataTypeKey, not both. Leave one of them NULL.
    /// </summary>
    Guid? DataTypeKey { get; }

    IEnumerable<Guid> CategoryIds { get; }
    IEnumerable<IContentBlockLayout> Layouts { get; }
    IEnumerable<string> LimitToDocumentTypes { get; }
    IEnumerable<string> LimitToCultures { get; }
}
