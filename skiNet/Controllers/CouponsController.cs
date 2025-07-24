using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CouponsController : BaseApiController
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<AppCoupon>> ValidateCoupon(string code)
        {
            var coupon = await _couponService.GetCouponFromPromoCode(code);
            if (coupon == null) return BadRequest("Invalid voucher code");
            return coupon;
        }
    }
}
