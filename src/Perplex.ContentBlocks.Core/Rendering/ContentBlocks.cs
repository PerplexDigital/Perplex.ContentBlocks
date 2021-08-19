using System.Collections.Generic;

namespace Perplex.ContentBlocks.Rendering
{
    public class ContentBlocks : IContentBlocks
    {
        public static readonly IContentBlocks Empty = new ContentBlocks();

        public IContentBlockViewModel Header { get; set; }

        public IEnumerable<IContentBlockViewModel> Blocks { get; set; }
    }
}
