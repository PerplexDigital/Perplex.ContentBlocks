namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlocksInterValue
{
    public ContentBlockInterValue? Header { get; set; }

    public IEnumerable<ContentBlockInterValue> Blocks { get; set; }
        = Array.Empty<ContentBlockInterValue>();
}
