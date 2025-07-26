using CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace API.RequestHelpers
{
    [AttributeUsage(AttributeTargets.All)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        //context - przed wykonaniem akcji czyli metody w kontreolerze, next - po wykonaniu akcji
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Przed wykonaniem akcji - próbujemy pobrać z cache
            var cacheService = context.HttpContext.RequestServices
                .GetRequiredService<IResponseCacheService>();

            // Tworzy unikalny klucz na podstawie aktualnego requesta (URL + parametry query)
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            // Próbuje pobrać odpowiedź z cache na podstawie wygenerowanego klucza
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            // Jeśli jest coś w cache
            // Zwraca odpowiedź z cache, bez wykonywania dalszej części akcji kontrolera
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;

                return;
            }

            // Jeśli nie ma nic w cache, wywołuje akcję kontrolera
            var executedContext = await next();

            // Jeśli odpowiedź akcji to OkObjectResult (czyli status 200 i jakiś obiekt)
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                if(okObjectResult.Value != null)
                {
                    // Zapisuje do cache odpowiedź z akcji pod stworzonym wcześniej kluczem,
                    // na określony czas (TimeToLiveSeconds
                    await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, 
                        TimeSpan.FromSeconds(_timeToLiveSeconds));
                }
            }
        }

        // Buduje klucz cache na podstawie ścieżki i parametrów zapytania   
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            foreach(var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
