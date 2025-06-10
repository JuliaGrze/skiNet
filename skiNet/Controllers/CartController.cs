using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Handles shopping cart API requests
    public class CartController : BaseApiController
    {
        private readonly ICartService _cartService;

        // Injects cart service (handles business logic for the cart)
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Retrieves the shopping cart for a given id.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
        {
            var cart = await _cartService.GetCartAsync(id);
            return Ok(cart ?? new ShoppingCart { Id = id });
        }

        /// <summary>
        /// Creates or updates the shopping cart.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
        {
            var updatedCart = await _cartService.SetCartAsync(cart);
            if (updatedCart == null) return BadRequest("Problem with cart");
            return updatedCart;
        }

        /// <summary>
        /// Deletes the shopping cart for a given id.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult> DeleteCart(string id)
        {
            var result = await _cartService.DeleteTaskAsync(id);
            if (!result) return BadRequest("Problem deleting cart");
            return Ok();
        }
    }

}
