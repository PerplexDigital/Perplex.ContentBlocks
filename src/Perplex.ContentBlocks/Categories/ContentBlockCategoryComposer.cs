using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Categories
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Install)]
    public class ContentBlockCategoriesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IContentBlockCategoryRepository, InMemoryContentBlockCategoryRepository>();
        }
    }
}
