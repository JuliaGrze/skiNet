using CORE.Entities;
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
    public class CartService : ICartService
    {
        private readonly IDatabase _database; //reprezentuje pojedynczą bazę danych Redisa

        public CartService(IConnectionMultiplexer redis) //to jest połączenie z serwerem Redis — singleton
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteCartAsync(string key)
        {
            // Removes the cart with the given key from Redis
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<ShoppingCart?> GetCartAsync(string key)
        {
            // Gets the cart with the given key from Redis
            var data = await _database.StringGetAsync(key);

            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart?>(data!);
        }

        public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
        {
            var created = await _database.StringSetAsync(
                    cart.Id,
                    JsonSerializer.Serialize(cart),
                    TimeSpan.FromDays(30) // Cart expires after 30 days
                );
            
            if(!created) return null;

            return await GetCartAsync(cart.Id);
        }
    }
}
