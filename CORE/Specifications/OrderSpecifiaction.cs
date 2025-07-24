using CORE.Entities.OrderAggreagte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    //to filtr warunków (czyli specyfikacja), który pozwala dynamicznie określać, jakie dane z bazy mają zostać pobrane
    public class OrderSpecifiaction : BaseSpecification<Order>
    {
        //„Stwórz specyfikację, która wybiera tylko te zamówienia (Order), których BuyerEmail jest równy podanemu emai
        public OrderSpecifiaction(string email) : base(x => x.BuyerEmail == email) 
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            AddOrderByDescending(x => x.OrderDate);
        }

        // Pobiera konkretne zamówienie użytkownika na podstawie jego ID i adresu e-mail 
        public OrderSpecifiaction(string email, int id) : base(x => x.BuyerEmail == email && x.Id == id) 
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        //filtruje zamówienia po PaymentIntentId
        public OrderSpecifiaction(string paymentIntentId, bool isPaymentIntent) 
            : base(x => x.PaymentIntentId == paymentIntentId) 
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        //Pozwala pobierać zamówienia według statusu, ze stronicowaniem i sortowaniem.
        public OrderSpecifiaction(OrderSpecParams specParams) 
            : base(x => string.IsNullOrEmpty(specParams.Status) || x.Status == ParseStatus(specParams.Status))
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            AddOrderByDescending(x => x.OrderDate);
        }

        //Pobiera jedno konkretne zamówienie po jego ID (np. do szczegółów zamówienia)
        public OrderSpecifiaction(int id) : base(x => x.Id == id) 
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        //Służy do konwersji tekstowego statusu (np. „paid”) na enum OrderStatus (np. OrderStatus.Paid)
        private static OrderStatus? ParseStatus(string status)
        {
            if (Enum.TryParse<OrderStatus>(status, true, out var result))  
                return result ;
            return null ;
        }  
    }
}
