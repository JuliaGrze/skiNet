using CORE.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        //reprezentuje polaczenie z serwisem Redis
        //Służy do nawiązywania i zarządzania połączeniem z bazą Redis
        private readonly IConnectionMultiplexer _redis;
        //To interfejs reprezentujący konkretną „bazę danych” (database) w Redisie
        //Służy do wykonywania operacji na danej bazie, np. ustawianie wartości, pobieranie, usuwanie kluczy itp
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase(1); 
        }

        /// <summary>
        /// Serializuje i cache’uje odpowiedź w Redisie na określony czas.
        /// </summary>
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            //camelCase (np. productName zamiast ProductName)
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            // np: { ProductName = "Narty" } -> {"productName":"Narty"}
            var serializedResponse = JsonSerializer.Serialize(response, options);

            //Asynchronicznie zapisuje powstały string (serializedResponse) do bazy Redis pod kluczem cacheKey
            //Dodaje ustawiony czas życia (timeToLive), po którym Redis sam usunie te dane
            //Jeśli coś już było pod tym kluczem — zostanie nadpisane
            await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
        }

        /// <summary>
        /// Pobiera z cache odpowiedź pod wskazanym kluczem. Zwraca string lub null.
        /// </summary>
        public async Task<string?> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);

            if(cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;
        }

        /// <summary>
        /// Usuwa wszystkie klucze z Redis, które zawierają podany wzorzec (pattern) w nazwie.
        /// Służy do inwalidacji cache np. po dodaniu, edycji lub usunięciu produktu.
        /// </summary>
        public async Task RemoveCacheByPatttern(string pattern)
        {
            // Pobiera pierwszy endpoint serwera Redis (adres IP i port)
            var server = _redis.GetServer(_redis.GetEndPoints().First());

            var keys = server.Keys(database: 1, pattern: $"*{pattern}*").ToArray();

            // Jeśli są znalezione jakieś klucze
            if (keys.Length != 0)
            {
                // Usuwa je asynchronicznie z Redis 
                await _database.KeyDeleteAsync(keys);
            }
                
        }
    }
}
