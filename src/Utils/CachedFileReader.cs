using System;
using System.IO;
using System.Text;

namespace Perplex.ContentBlocks.Utils
{
    public class CachedFileReader : ICachedFileReader
    {
        private bool _started;
        private readonly IFileWatcher _watcher;
        private string _fileContents;
        private string _filePath;

        public CachedFileReader(IFileWatcher watcher)
        {
            _watcher = watcher;
        }

        public void Start(string filePath, Action onChange)
        {
            if (_started)
            {
                // DK | 2019-04-19
                // Momenteel nog niet ondersteund om te stoppen, dus ook niet
                // om opnieuw te starten als je al gestart bent want
                // de huidige watcher moet dan eerst worden opgeruimd.
                // Niet moeilijk, maar daar is momenteel geen code voor want
                // dit gebruiken we niet.
                throw new NotSupportedException("Already started. Cannot call Start() multiple times at this time");
            }

            _filePath = filePath;

            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            _watcher.Watch(directory, fileName, NotifyFilters.LastWrite, () =>
            {
                UpdateFileContents(filePath);
                onChange?.Invoke();
            });

            _started = true;
        }

        private string ReadFileContentsFromDisk(string filePath)
        {
            try
            {
                // From: https://stackoverflow.com/a/9760751
                // Dit omdat File.ReadAllText() sneller een exception gooit
                // als een ander programma nog iets schrijft, en met de FileWatcher
                // triggeren events soms 2x als een programme iets wegschrijft in 2 acties.
                // Zie ook https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (IOException)
            {
                return null;
            }
        }

        public string GetFileContents()
        {
            if (_fileContents == null)
            {
                UpdateFileContents(_filePath);
            }

            return _fileContents;
        }

        private void UpdateFileContents(string filePath)
        {
            _fileContents = ReadFileContentsFromDisk(filePath);
        }
    }
}
