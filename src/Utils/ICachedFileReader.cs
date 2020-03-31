using System;

namespace Perplex.ContentBlocks.Utils
{
    public interface ICachedFileReader
    {
        string GetFileContents();

        void Start(string filePath, Action onChange);
    }
}
