using System.Collections.Concurrent;

namespace ImageProcessingApi.Services
{
    public interface ICacheService
    {
        byte[] Get(string key);
        void Set(string key, byte[] data);
    }

    public class CacheService : ICacheService
    {
        // Creates a thread-safe dictionary to store cached items,
        // where the key is a string (e.g., image hash + filter) and the value is the processed image (as a byte array).
        private readonly ConcurrentDictionary<string, byte[]> _cache = new();


        public byte[] Get(string key)
        {
            return _cache.TryGetValue(key, out var cachedData) ? cachedData : null;
        }

        public void Set(string key, byte[] data)
        {
            _cache[key] = data;
        }

    }
}