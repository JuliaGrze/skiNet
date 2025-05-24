using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    //Builds LINQ query logic based on data from Product Specification Params
    public class ProductSpecification : BaseSpecification<Product>
    {
        // Używa BaseSpecification() bez żadnego filtra (Criteria == null)
        // np. do sortowania, paginacji
        public ProductSpecification() : base() { }

        //Umożliwia przekazanie dowolnego filtru z zewnątrz.
        public ProductSpecification(Expression<Func<Product, bool>> criteria) : base(criteria) { }
        public ProductSpecification(ProductSpecificationParams specificationParams) : base(x =>
            (string.IsNullOrEmpty(specificationParams.Search) || x.Name.ToLower().Contains(specificationParams.Search)) &&
            (!specificationParams.Brands.Any() || specificationParams.Brands.Contains(x.Brand)) &&
            (!specificationParams.Types.Any() || specificationParams.Types.Contains(x.Type)))
        {
            //pagging
            ApplyPaging(specificationParams.PageSize * (specificationParams.PageIndex - 1), specificationParams.PageSize);

            //sorting
            switch (specificationParams.Sort)
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
