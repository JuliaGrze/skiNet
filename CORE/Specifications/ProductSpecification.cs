using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        // Używa BaseSpecification() bez żadnego filtra (Criteria == null)
        // np. do sortowania, paginacji
        public ProductSpecification() : base() { }

        //Umożliwia przekazanie dowolnego filtru z zewnątrz.
        public ProductSpecification(Expression<Func<Product, bool>> criteria) : base(criteria) { }
        public ProductSpecification(string? brand, string? type, string? sort) : base(x =>
            (string.IsNullOrWhiteSpace(brand) || x.Brand == brand) &&
            (string.IsNullOrWhiteSpace(type) || x.Type == type))
        {
            switch (sort)
            {
                case "priceAsc":
                    AddOrderBy(x => x.Price);
                    break;
                case "priceDesc":
                    AddOrderByDescending(x => x.Price);
                    break;
                default:
                    AddOrderBy(x => x.Name);
                    break;
            }
        }
    }
}
