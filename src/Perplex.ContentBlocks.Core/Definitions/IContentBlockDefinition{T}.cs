namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockDefinition<out T> : IContentBlockDefinition where T : IContentBlockViewComponent
{
}
