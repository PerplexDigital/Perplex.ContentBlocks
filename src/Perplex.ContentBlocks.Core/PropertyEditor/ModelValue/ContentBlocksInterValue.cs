using System.Collections.Generic;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
    public class ContentBlocksInterValue
    {
        public ContentBlockInterValue Header { get; set; }
        public List<ContentBlockInterValue> Blocks { get; set; }
    }
}
