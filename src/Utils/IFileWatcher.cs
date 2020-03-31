using System;
using System.IO;

namespace Perplex.ContentBlocks.Utils
{
    public interface IFileWatcher
    {
        void Dispose();

        void Watch(string directory, string fileFilter, NotifyFilters notifyFilters, Action callback = null);
    }
}
