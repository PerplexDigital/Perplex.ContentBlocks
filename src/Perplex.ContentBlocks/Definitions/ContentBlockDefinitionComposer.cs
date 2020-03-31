using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Definitions
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockDefinitionComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IContentBlockDefinitionRepository, InMemoryContentBlockDefinitionRepository>(Lifetime.Singleton);
            composition.Register<IContentBlockDefinitionFilterer, ContentBlockDefinitionFilterer>(Lifetime.Singleton);
        }
    }
}
