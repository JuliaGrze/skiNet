using CORE.Entities.OrderAggreagte;
using CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.Extensions;
using CORE.Entities;
using CORE.Specifications;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {   
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        public OrdersController(ICartService cartService, IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _unitOfWork = unitOfWork;
        }
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            //Pobiera e-mail zalogowanego użytkownika z tokena JWT
            var email = User.GetEmail();

            //Pobieramy koszyk użytkownika z Redis po id
            var cart = await _cartService.GetCartAsync(orderDto.CartId);

            if (cart == null)
                return BadRequest("Cart not found");

            //sprawdzamy czy była inicjowana płatność
            if (cart.PaymentIntentId == null)
                return BadRequest("No payment intent for this order");

            var items = new List<OrderItem>();

            foreach(var item in cart.Items)
            {
                //Pobieramy z bazy danych aktualne dane o produkcie (Product), by np. upewnić się, że cena lub dostępność są aktualne.
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);

                if (productItem == null)
                    return BadRequest("Problem with the order");

                //To tzw. value object, który zawiera dane o produkcie w czasie zamówienia
                //(snapshot – bo mogą się zmienić w przyszłości).
                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl,
                };

                //Tworzymy pozycję zamówienia i dodajemy do listy items
                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = item.Price,
                    Quantity = item.Quantity,
                };

                items.Add(orderItem);
            }

            //pobranie metody dostawy
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);

            if (deliveryMethod == null)
                return BadRequest("No delivery method selected");

            var order = new Order
            {
                BuyerEmail = email,
                ShippingAddress = orderDto.ShippingAddress,
                DeliveryMethod = deliveryMethod,
                OrderItems = items,
                Subtotal = items.Sum(x => x.Price * x.Quantity),
                PaymentSummarry = orderDto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId
            };

            //dodanie zamowienia do bazy danych
            _unitOfWork.Repository<Order>().Add(order);

            //zapis zmian
            if(await _unitOfWork.Complete())
            {
                return order;
            }
            return BadRequest("Problem creating order");

        }

        [HttpGet]
        // Zwraca liste wsyztkich zamowien danego uzytkownika
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var spec = new OrderSpecifiaction(User.GetEmail());

            var orders = await _unitOfWork.Repository<Order>().ListAsyncWIithSpec(spec);

            var ordersToReturn = orders.Select(x => x.ToDto()).ToList();

            return Ok(ordersToReturn);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecifiaction(User.GetEmail(), id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return NotFound();

            return order.ToDto();
        }
    }
}
