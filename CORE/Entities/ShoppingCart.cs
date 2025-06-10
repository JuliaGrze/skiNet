using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    /// <summary>
    /// Represents a user's shopping cart entity that contains a unique identifier and list of items added to the cart.
    /// </summary>
    public class ShoppingCart
    {
        public required string Id { get; set; }
        public List<CartItem> Items { get; set; } = [];
    }
}
