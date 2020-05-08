using System.Collections.Generic;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
    public class ContentBlocksModelValue
    {
        public int Version { get; set; }

        public ContentBlockModelValue Header { get; set; }

        public List<ContentBlockModelValue> Blocks { get; set; }
    }
}
