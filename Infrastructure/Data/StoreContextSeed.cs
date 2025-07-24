using CORE.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, UserManager<AppUser> userManager)
        {
            //sprawdzmay czy mamy admina w bazie danych
            if (!userManager.Users.Any(x => x.UserName == "admin@test.com"))
            {
                var user = new AppUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };

                //zapisanie admina do bazy danych
                await userManager.CreateAsync(user, "Pa$$w0rd");
                //Nadanie roli Admin
                await userManager.AddToRoleAsync(user, "Admin");
            }  

            //To daje folder, gdzie jest aktualny plik DLL aplikacji
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


            //chceck if Products table isn't empty
            if (!context.Products.Any())
            {
                var productsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                //ensure that deserialize went complete
                if (products == null) return;

                context.Products.AddRange(products);

                await context.SaveChangesAsync();
            }

            //chceck if DeliveryMethods table isn't empty
            if (!context.DeliveryMethods.Any())
            {
                var deliveryData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");

                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                //ensure that deserialize went complete
                if (deliveryMethods == null) return;

                context.DeliveryMethods.AddRange(deliveryMethods);

                await context.SaveChangesAsync();
            }
        }
    }
}
