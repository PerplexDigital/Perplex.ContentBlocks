using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Umbraco.ModelValue
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksModelValueComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ContentBlocksModelValueDeserializer>(Lifetime.Singleton);
        }
    }
}
