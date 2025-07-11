using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Gdy ktoś wpisze adres, który nie jest trasą API (np. /produkty/5), a serwer nie zna tej trasy
    // to ASP.NET Core przekieruje to żądanie do FallbackController – metoda Index
    public class FallbackController : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}
