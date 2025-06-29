using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    public interface IPaymentService
    {
        //Metoda tworzy nowy PaymentIntent (zamiar płatności) albo aktualizuje istniejący, bazując na koszyku użytkownika o ID cartId
        Task<ShoppingCart> CreateOrUpdatePaymentIntent(string cartId);
    }
}
