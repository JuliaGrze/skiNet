using CORE.Entities;
using CORE.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = CORE.Entities.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepository;

        public PaymentService(IConfiguration configuration, ICartService cartService,
           IGenericRepository<Product> productRepository, IGenericRepository<DeliveryMethod> deliveryMethodRepository)
        {
            _configuration = configuration;
            _cartService = cartService;
            _productRepository = productRepository;
            _deliveryMethodRepository = deliveryMethodRepository;

            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
        }

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
            // Pobierz koszyk użytkownika na podstawie jego ID
            var cart = await _cartService.GetCartAsync(cartId);
            if (cart == null)
                return null;  // Jeśli koszyk nie istnieje, zwróć null

            // Zmienna do przechowywania kosztu dostawy
            var shippingPrice = 0m;

            // Jeśli wybrano metodę dostawy, pobierz jej cenę
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _deliveryMethodRepository.GetByIdAsync((int)cart.DeliveryMethodId);

                if (deliveryMethod == null)
                    return null; // Jeśli metoda dostawy nie istnieje, zakończ

                shippingPrice = deliveryMethod.Price;
            }

            // Zaktualizuj ceny produktów w koszyku (jeśli się zmieniły od czasu dodania do koszyka)
            foreach (var item in cart.Items)
            {
                var productItem = await _productRepository.GetByIdAsync(item.ProductId);

                if (productItem == null) return null;

                if (item.Price != productItem.Price)
                {
                    //  Synchronizujemy cenę w koszyku z aktualną ceną produktu
                    item.Price = productItem.Price;
                }
            }

            // Stripe SDK – do zarządzania płatnościami
            var service = new PaymentIntentService();
            PaymentIntent? intent = null;

            // wspólna kwota całkowita (w groszach)
            var amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100))
                         + (long)(shippingPrice * 100);

            // Jeśli koszyk nie ma jeszcze przypisanego PaymentIntent – utwórz nowy
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount, // suma pozycji w groszach + dostawa
                    Currency = "pln", // Waluta: złotówki
                    PaymentMethodTypes = ["card"] // Obsługiwane metody płatności (na razie tylko karta)
                };

                intent = await service.CreateAsync(options); // Utwórz nowy PaymentIntent w Stripe

                // Zapisz dane zwrócone przez Stripe w koszyku
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else // Jeśli PaymentIntent już istnieje – zaktualizuj jego kwotę
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount
                };

                try
                {
                    intent = await service.UpdateAsync(cart.PaymentIntentId, options);
                }
                catch (StripeException ex) when (ex.Message.Contains("No such payment_intent"))
                {
                    // Jeśli Stripe nie zna ID – tworzymy nowy intent od zera
                    var createOptions = new PaymentIntentCreateOptions
                    {
                        Amount = amount,
                        Currency = "pln",
                        PaymentMethodTypes = ["card"]
                    };

                    intent = await service.CreateAsync(createOptions);
                    cart.PaymentIntentId = intent.Id;
                    cart.ClientSecret = intent.ClientSecret;
                }
            }

            // Zapisz zaktualizowany koszyk (np. do Redis)
            await _cartService.SetCartAsync(cart);

            return cart;
        }

    }
}
