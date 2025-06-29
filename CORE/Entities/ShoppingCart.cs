using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    /// <summary>
    /// Represents a shopping cart with items, delivery method, and Stripe payment data.
    /// </summary>
    public class ShoppingCart
    {
        public required string Id { get; set; }
        public List<CartItem> Items { get; set; } = [];
        public int? DeliveryMethodId { get; set; }
        // 🔐 Stripe Payment Integration
        public string? ClientSecret { get; set; } //string returned by Stripe
        public string? PaymentIntentId { get; set; }

    }
}
