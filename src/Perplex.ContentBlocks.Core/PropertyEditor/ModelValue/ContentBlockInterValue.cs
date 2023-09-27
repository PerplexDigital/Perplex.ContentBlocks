using Newtonsoft.Json.Linq;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlockInterValue
{
    public Guid Id { get; set; }
    public Guid DefinitionId { get; set; }
    public Guid LayoutId { get; set; }
    public JArray? Content { get; set; }
}
