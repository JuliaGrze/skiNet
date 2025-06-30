using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities.OrderAggreagte
{
    /// <summary>
    /// Payment details snapshot stored with the order.
    /// </summary>
    public class PaymentSummary
    {
        public int Last4  { get; set; }
        public string Brand { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
    }
}
