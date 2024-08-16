

using Infrastructure.Models;

namespace Infrastructure.Utils
{
    public static class MemoryCacheUtil
    {
        public static string GetCacheKey(SeoRequest seoRequest)
        {
            return $"{seoRequest.SearchProvider}_{seoRequest.Url}_{seoRequest.SearchPhrase}";
        }
    }
}

