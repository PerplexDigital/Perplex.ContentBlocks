using System.Collections.Generic;

namespace Perplex.ContentBlocks.Umbraco.ModelValue
{
    public class ContentBlocksModelValue
    {
        public int Version { get; set; }

        public ContentBlockModelValue Header { get; set; }

        public List<ContentBlockModelValue> Blocks { get; set; }
    }
}
