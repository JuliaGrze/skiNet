using API.Extensions;
using API.SignalR;
using CORE.Entities;
using CORE.Entities.OrderAggreagte;
using CORE.Interfaces;
using CORE.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly string _whSecret = "";

        public PaymentsController(IPaymentService paymentService, IUnitOfWork unitOfWork, 
            ILogger<PaymentsController> logger, IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _hubContext = hubContext;

            _whSecret = _configuration["StripeSettings:WhSecret"];
        }

       /// <summary>
       /// Creates or updates a Stripe PaymentIntent for the given cart ID.
       /// </summary>
       /// <param name="cartId">The ID of the shopping cart</param>
       /// <returns>Updated shopping cart with PaymentIntent info</returns>
       [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await _paymentService.CreateOrUpdatePaymentIntent(cartId);

            if (cart == null) return BadRequest("Problem with your cart");

            return Ok(cart);
        }

        /// <summary>
        /// Returns a list of available delivery methods for shipping.
        /// </summary>
        /// <returns>A read-only list of delivery method options</returns>
        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await _unitOfWork.Repository<DeliveryMethod>().GetListAsync());
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            // 1. Odczytuje całe ciało żądania jako string JSON (to dane webhooka od Stripe)
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                // 2. Parsuje JSON na obiekt zdarzenia Stripe (Event) i weryfikuje jego podpis
                var stripeEvent = ConstructStripeEvent(json);

                // 3. Sprawdza, czy obiekt zdarzenia to PaymentIntent
                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    // Zwraca błąd, jeśli nie jest PaymentIntent
                    return BadRequest("Invalid event data");
                }

                // 4. Obsługuje zdarzenie pomyślnej płatności
                await HandlePaymentIntentSucceeded(intent);

                // Jeśli wszystko się powiodło, zwracamy 200 OK.
                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpeceted error data");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpeceted error data");
            }
        }

            private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
            {
                // Sprawdza, czy status płatności to "succeeded"
                if (intent.Status == "succeeded")
                {
                    // Tworzy specyfikację do wyszukania zamówienia po PaymentIntentId(unikalny identyfikator płatności)
                    var spec = new OrderSpecifiaction(intent.Id, true);

                    // Pobiera zamówienie z bazy według specyfikacji
                    var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec)
                        ?? throw new Exception("Order not found");

                    // Porównuje kwotę zamówienia z kwotą z PaymentIntent
                    // (obliczona kwota * 100 bo Stripe podaje w groszach)
                    if ((long) order.GetTotal() * 100 != intent.Amount)
                    {
                        // Jeśli się nie zgadza — oznacza problem z płatnością
                        order.Status = OrderStatus.PaymentMismatch;
                    } else
                    {
                        order.Status = OrderStatus.PaymentReceive;
                    }

                    // Zapisuje zmiany w bazie
                    await _unitOfWork.Complete();

                    // Pobiera ConnectionId użytkownika po emailu, aby wysłać mu powiadomienie na frontend
                    var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

                    if (!string.IsNullOrEmpty(connectionId))
                    {
                        // Wysyła powiadomienie do klienta frontendowego z danymi zamówienia (DTO)
                        await _hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
                    }
                }
            }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                // Parsuje event i jednocześnie weryfikuje autentyczność podpisu webhooka Stripe,
                // korzystając z tajnego klucza webhooka (_whSecret) oraz nagłówka "Stripe-Signature"
                return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
            }
            catch (Exception ex)
            {
                // Loguje błąd
                _logger.LogError(ex, "Failed to construct stripe event"); 

                // Rzuca wyjątek, jeśli podpis jest niepoprawny
                throw new StripeException("Invalid signature");
            }
        }
    }
}
