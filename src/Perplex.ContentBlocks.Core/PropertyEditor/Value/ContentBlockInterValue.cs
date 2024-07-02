using Umbraco.Cms.Core.Models.Blocks;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlockInterValue
{
    public Guid Id { get; set; }
    public Guid DefinitionId { get; set; }
    public Guid LayoutId { get; set; }
    public BlockItemData? Content { get; set; }
}
