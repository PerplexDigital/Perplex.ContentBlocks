using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions
{
    public interface IContentBlockDefinition
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        string PreviewImage { get; }
        int? DataTypeId { get; }
        Guid? DataTypeKey { get; }

        IEnumerable<Guid> CategoryIds { get; }
        IEnumerable<IContentBlockLayout> Layouts { get; }
        IEnumerable<string> LimitToDocumentTypes { get; }
        IEnumerable<string> LimitToCultures { get; }
    }
}
