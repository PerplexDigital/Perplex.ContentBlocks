using System.Text.Json.Nodes;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlockInterValue
{
    public Guid Id { get; set; }
    public Guid DefinitionId { get; set; }
    public Guid LayoutId { get; set; }
    public JsonNode? Content { get; set; }
}
