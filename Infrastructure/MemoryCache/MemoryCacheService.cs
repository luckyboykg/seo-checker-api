using Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.MemoryCache
{
    public interface IMemoryCacheService
    {
        bool TryGetValue(string cacheKey, out SeoInfo? cachedValue);
        SeoInfo? Set(string cacheKey, SeoInfo? cachedValue, TimeSpan absoluteExpirationRelativeToNow);
    }

    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetValue(string cacheKey, out SeoInfo? cachedValue)
        {
            if (_memoryCache.TryGetValue(cacheKey, out SeoInfo? value))
            {
                cachedValue = value;
                return true;
            }

            cachedValue = null;
            return false;
        }

        public SeoInfo? Set(string cacheKey, SeoInfo? cachedValue, TimeSpan absoluteExpirationRelativeToNow)
        {
            return _memoryCache.Set(cacheKey, cachedValue, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            });
        }
    }
}

