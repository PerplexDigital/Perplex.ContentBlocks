using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Upgrade)]
    public class ContentBlocksModelValueComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ContentBlocksModelValueDeserializer>(Lifetime.Singleton);
        }
    }
}
