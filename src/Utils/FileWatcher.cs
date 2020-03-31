using System;
using System.IO;

namespace Perplex.ContentBlocks.Utils
{
    // See: https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netframework-4.7.2
    // DK | 2019-This attribute might be necessary, but seems to work fine without so we will uncomment it when running into
    // issues, perhaps on the live environment?
    // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class FileWatcher : IDisposable, IFileWatcher
    {
        private FileSystemWatcher _watcher;
        private Action _callback;
        private bool _watching;

        public void Watch(string directory, string fileFilter, NotifyFilters notifyFilters, Action callback = null)
        {
            if (_watching)
            {
                // DK | 2019-04-19
                // Momenteel nog niet ondersteund om een nieuwe watch te starten.
                // Dit is niet echt complex maar ik heb nog geen code gemaakt
                // om een huidige watch op te ruimen / te stoppen.
                throw new NotSupportedException("Already watching. Cannot call Watch() multiple times at this time");
            }

            _callback = callback;

            _watcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = fileFilter,
                NotifyFilter = notifyFilters,
            };

            _watcher.Renamed += WatcherCallback;
            _watcher.Changed += WatcherCallback;
            _watcher.Created += WatcherCallback;
            _watcher.Deleted += WatcherCallback;

            _watcher.EnableRaisingEvents = true;

            _watching = true;
        }

        ~FileWatcher()
        {
            Dispose();
        }

        private void WatcherCallback(object _, object __) =>
            _callback?.Invoke();

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Renamed -= WatcherCallback;
                _watcher.Changed -= WatcherCallback;
                _watcher.Created -= WatcherCallback;
                _watcher.Deleted -= WatcherCallback;
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }
        }
    }
}
