using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Utils
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class ContentBlockUtilsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ContentBlockUtils>(Lifetime.Singleton);
        }
    }
}
