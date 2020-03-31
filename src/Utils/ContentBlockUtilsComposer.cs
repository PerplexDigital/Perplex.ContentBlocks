using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Utils
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlockUtilsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ICachedFileReader, CachedFileReader>();
            composition.Register<IFileWatcher, FileWatcher>();

            composition.Register(typeof(CachedJsonFileReader<>));
            composition.Register(typeof(CachedJsonFileReader<,>));
        }
    }
}
