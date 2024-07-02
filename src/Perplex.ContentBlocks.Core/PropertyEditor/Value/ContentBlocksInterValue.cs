namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksInterValue
{
    public ContentBlockInterValue? Header { get; set; }

    public IEnumerable<ContentBlockInterValue> Blocks { get; set; }
        = Array.Empty<ContentBlockInterValue>();
}
