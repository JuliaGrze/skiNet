using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    /// <summary>
    /// Interface for a response caching service.
    /// Provides methods to store, retrieve, and remove cached responses by key or pattern.
    /// </summary>
    public interface IResponseCacheService
    {
        //Zapisuje (cache’uje) odpowiedź (response) pod danym kluczem (cacheKey) na określony czas (timeToLive)
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);

        //Odczytuje z cache odpowiedź zapisaną pod danym kluczem (cacheKey).
        Task<string?> GetCachedResponseAsync(string cacheKey);

        //Usuwa z cache wszystkie wpisy, których klucz pasuje do wzorca (pattern)
        Task RemoveCacheByPatttern(string pattern);
    }
}
