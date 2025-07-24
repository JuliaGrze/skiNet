using API.DTO;
using API.Extensions;
using CORE.Entities.OrderAggreagte;
using CORE.Interfaces;
using CORE.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Endpointy dostępne tylko dla użytkowników z rolą "Admin"
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private IUnitOfWork _unitOfWork;
        private IPaymentService _paymentService;

        public AdminController(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        // Endpoint GET: /admin/orders
        // Zwraca listę zamówień według zadanych parametrów (np. status, strona, rozmiar strony)
        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery]OrderSpecParams specParams)
        {
            // Tworzy specyfikację (filtr i instrukcje, jakie dane pobrać)
            var spec = new OrderSpecifiaction(specParams);

            // Wywołuje generyczną metodę do pobierania stronowanych wyników z repozytorium
            return await CreatePageResult(_unitOfWork.Repository<Order>(), spec, 
                specParams.PageIndex, specParams.PageSize, o => o.ToDto());
        }

        // Endpoint GET: /admin/orders/{id}
        // Zwraca listę zamówień według podanego id zamówienia
        [HttpGet("orders/{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            // Tworzyy specyfikację - czyli filtr pobierający zamówienie o konkretnym ID
            var spec = new OrderSpecifiaction(id);

            // Pobieramy zamówienie z repozytorium na podstawie specyfikacji
            // Jeśli nie ma takiego zamówienia, order będzie null
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            // Jeśli nie znaleziono zamówienia, zwracamy błąd 400 i komunikat.
            if (order == null) return BadRequest("No order with that id");

            // Jeśli zamówienie istnieje, zamieniamy je na DTO i zwracamy
            return order.ToDto();
        }

        // Endpoint POST: /admin/orders/refund{id}
        // Umożliwia zwrot pieniędzy (refund) za wybrane zamówienie (o danym ID)
        [HttpPost("orders/refund/{id:int}")]
        public async Task<ActionResult<OrderDto>> RefundOrder(int id)
        {
            // Tworzymy specyfikację, by pobrać zamówienie o danym ID (wraz z powiązanymi danymi)
            var spec = new OrderSpecifiaction(id);

            // Pobieramy zamówienie z repozytorium według specyfikacji
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            // Jeśli nie znaleziono zamówienia, zwracamy błąd 400 z komunikatem
            if (order == null) return BadRequest("No order with thatt id");

            // Jeśli status zamówienia to 'Pending', znaczy że nie otrzymano jeszcze płatności
            // i nie ma czego zwracać — od razu kończymy z błędem
            if (order.Status == OrderStatus.Pending)
                return BadRequest("Payment not received for this order");

            // Wywołujemy refundację płatności przez serwis płatności (np. Stripe)
            // Przekazujemy PaymentIntentId powiązany z zamówieniem
            var result = await _paymentService.RefundPayment(order.PaymentIntentId);

            // Sprawdzamy, czy refundacja się powiodła
            if (result == "succeeded")
            {
                //  Jeśli tak, zmieniamy status zamówienia na 'Refunded'
                order.Status = OrderStatus.Refunded;

                // Zapisujemy zmiany w bazie danych
                await _unitOfWork.Complete();

                // Zwracamy DTO zamówienia po refundzie
                return order.ToDto();
            }

            // Jeśli refundacja nie powiodła się, zwracamy błąd 400 z komunikatem
            return BadRequest("problem refunding order");
        }
    }
}
