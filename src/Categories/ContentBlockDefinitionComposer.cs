using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Categories
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockCategoriesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IContentBlockCategoryService, HardCodedContentBlockCategoryService>(Lifetime.Singleton);
        }
    }
}
