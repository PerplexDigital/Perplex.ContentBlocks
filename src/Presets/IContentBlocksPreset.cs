using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Presets
{
    public interface IContentBlocksPreset
    {
        Guid Id { get; }
        string Name { get; }

        IEnumerable<string> ApplyToCultures { get; }
        IEnumerable<string> ApplyToDocumentTypes { get; }

        IContentBlockPreset Header { get; }
        IEnumerable<IContentBlockPreset> Blocks { get; }
    }
}
