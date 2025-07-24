using CORE.Entities;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.Interfaces;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service for handling operations related to Stripe promotion codes and coupons.
    /// Provides functionality to retrieve coupon information based on a given promo code.
    /// </summary>
    public class CouponService : ICouponService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponService"/> class
        /// and sets the Stripe API key from the application configuration.
        /// </summary>
        /// <param name="config">Application configuration containing Stripe settings.</param>
        public CouponService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        }

        /// <summary>
        /// Retrieves coupon details from Stripe based on the provided promotion code.
        /// </summary>
        /// <param name="code">The promotion code to search for.</param>
        /// <returns>
        /// An <see cref="AppCoupon"/> object containing coupon details if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
        {
            var promotionService = new PromotionCodeService();
            var options = new PromotionCodeListOptions
            {
                Code = code
            };
            var promotionCodes = await promotionService.ListAsync(options);
            var promotionCode = promotionCodes.FirstOrDefault();
            if (promotionCode != null && promotionCode.Coupon != null)
            {
                return new AppCoupon
                {
                    Name = promotionCode.Coupon.Name,
                    AmountOff = promotionCode.Coupon.AmountOff,
                    PercentOff = promotionCode.Coupon.PercentOff,
                    CouponId = promotionCode.Coupon.Id,
                    PromotionCode = promotionCode.Code
                };
            }
            return null;
        }
    }
}
