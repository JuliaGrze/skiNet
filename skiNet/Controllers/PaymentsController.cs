using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentsController(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
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
    }
}
