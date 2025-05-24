using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    public class BrandListSpecifications : BaseSpecification<Product, string>
    {
        public BrandListSpecifications()
        {
            AddSelect(x => x.Brand);
            AddDistinct();
        }
    }
}
