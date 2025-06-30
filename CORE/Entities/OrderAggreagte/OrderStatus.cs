using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities.OrderAggreagte
{
    /// <summary>
    /// Represents the current status of an order.
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        PaymentReceive,
        PaymentFailed

    }
}
