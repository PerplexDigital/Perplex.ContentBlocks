using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Providers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockProvidersComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IDocumentTypeAliasProvider, DocumentTypeAliasProvider>(Lifetime.Singleton);
        }
    }
}
