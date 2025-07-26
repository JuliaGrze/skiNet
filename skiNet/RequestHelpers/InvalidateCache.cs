using CORE.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers
{
    [AttributeUsage(AttributeTargets.All)]
    public class InvalidateCache : Attribute, IAsyncActionFilter
    {
        private readonly string _pattern;

        public InvalidateCache(string pattern)
        {
            _pattern = pattern;
        }

        /// <summary>
        /// Wywoływana po wykonaniu akcji. Jeśli nie było błędu, usuwa cache zgodnie ze wzorcem.
        /// </summary>
        /// <param name="context">Kontekst wykonania akcji (przed wywołaniem).</param>
        /// <param name="next">Delegat do wywołania akcji kontrolera.</param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Wywołuje akcję kontrolera (np. POST/PUT/DELETE) i czeka na jej zakończenie
            var resultContext = await next();

            // Jeśli akcja zakończyła się bez wyjątku lub wyjątek został obsłużony (nie doszło do errora 500 itp.)
            if (resultContext.Exception == null || resultContext.ExceptionHandled)
            {
                // Pobiera serwis cache z DI
                var cacheService = context.HttpContext.RequestServices
                    .GetRequiredService<IResponseCacheService>();

                // Usuwa cache pasujący do wzorca (np. "/api/products*")
                await cacheService.RemoveCacheByPatttern(_pattern);
            }
        }
    }
}
