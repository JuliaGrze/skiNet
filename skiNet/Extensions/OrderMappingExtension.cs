using API.DTO;
using CORE.Entities.OrderAggreagte;

namespace API.Extensions
{
    public static class OrderMappingExtension
    {
        //convert Order to OrderDto
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                BuyerEmail = order.BuyerEmail,
                ShippingAddress = order.ShippingAddress,
                DeliveryMethod = order.DeliveryMethod.Description,
                ShippingPrice = order.DeliveryMethod.Price,
                PaymentSummarry = order.PaymentSummarry,
                OrderItems = order.OrderItems.Select(x => x.ToDto()).ToList(),
                Subtotal = order.Subtotal,
                Totoal = order.GetTotal(),
                Status = order.Status.ToString(),
                PaymentIntentId = order.PaymentIntentId
            };
        }

        //convert OrderItem to OrderItemDto
        public static OrderItemDto ToDto(this OrderItem orderItem) 
        {
            return new OrderItemDto
            {
                ProductId = orderItem.Id,
                ProductName = orderItem.ItemOrdered.ProductName,
                PictureUrl = orderItem.ItemOrdered.PictureUrl,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity
            };
        }

    }
}
