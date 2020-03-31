using Newtonsoft.Json;
using System;

namespace Perplex.ContentBlocks.Utils
{
    public class CachedJsonFileReader<T> : CachedJsonFileReader<T, T>
    {
        public CachedJsonFileReader(ICachedFileReader cachedFileReader) : base(cachedFileReader)
        {
        }

        public void Watch(string filePath, Action onChange)
            => base.Watch(filePath, input => input, onChange);
    }

    public class CachedJsonFileReader<TSerializable, TDeserialized>
    {
        private ICachedFileReader _cachedFileReader;
        private Func<TSerializable, TDeserialized> _mapFn;

        public CachedJsonFileReader(ICachedFileReader cachedFileReader)
        {
            _cachedFileReader = cachedFileReader;
        }

        public virtual void Watch(string filePath, Func<TSerializable, TDeserialized> mapFn, Action onChange)
        {
            _cachedFileReader.Start(filePath, onChange);
            _mapFn = mapFn;
        }

        public TDeserialized GetData()
        {
            string json = _cachedFileReader.GetFileContents();
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            var data = JsonConvert.DeserializeObject<TSerializable>(json);
            return _mapFn.Invoke(data);
        }
    }
}
