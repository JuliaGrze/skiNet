    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace CORE.Entities.OrderAggreagte
    {
        /// <summary>
        /// Entity representing a single item in an order, including product info, price, and quantity.
        /// </summary>
        public class OrderItem : BaseEntity
        {
            public ProductItemOrdered ItemOrdered { get; set; } = null!;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
