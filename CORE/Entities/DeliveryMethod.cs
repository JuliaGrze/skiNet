﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Entities
{
    public class DeliveryMethod : BaseEntity
    {
        public required string ShortName { get; set; }
        public required string DeliveryTime { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
    }
}
