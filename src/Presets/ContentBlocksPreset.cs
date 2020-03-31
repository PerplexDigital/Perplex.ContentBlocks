using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Presets
{
    public class ContentBlocksPreset : IContentBlocksPreset
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        public IEnumerable<string> ApplyToCultures { get; set; }
            = Enumerable.Empty<string>();

        public IEnumerable<string> ApplyToDocumentTypes { get; set; }
            = Enumerable.Empty<string>();

        public IContentBlockPreset Header { get; set; }            

        public IEnumerable<IContentBlockPreset> Blocks { get; set; }
            = Enumerable.Empty<IContentBlockPreset>();
    }
}
