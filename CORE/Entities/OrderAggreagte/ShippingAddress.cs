﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities.OrderAggreagte
{
    /// <summary>
    /// Delivery address stored with the Order.
    /// </summary>
    public class ShippingAddress
    {
        public required string Name { get; set; }
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }
}
