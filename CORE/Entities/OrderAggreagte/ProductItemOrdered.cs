using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities.OrderAggreagte
{
    /// <summary>
    /// Value object embedded in Order; not a separate entity.
    /// </summary>
    public class ProductItemOrdered
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string PictureUrl { get; set; }

    }
}
