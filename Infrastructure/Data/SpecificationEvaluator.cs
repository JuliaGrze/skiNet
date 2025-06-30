using CORE.Entities;
using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    /// <summary>
    /// Provides functionality to apply a specification to a queryable entity set.
    /// This class is responsible for translating the filtering logic defined in a specification
    /// into a LINQ query that can be executed by Entity Framework.
    /// </summary>
    /// <typeparam name="T">The type of the entity, which must inherit from <see cref="BaseEntity"/>.</typeparam>
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        /// <summary>
        /// Applies a given specification to an <see cref="IQueryable{T}"/> by adding filtering and sorting logic.
        /// This includes applying a where clause (Criteria), ascending sort (OrderBy), and descending sort (OrderByDescending),
        /// based on the properties defined in the specification.
        /// </summary>
        /// <param name="query">The initial queryable source (e.g., <c>_context.Products</c>).</param>
        /// <param name="spec">The specification containing the filtering and sorting expressions.</param>
        /// <returns>The modified queryable with all applicable specification logic applied.</returns>


        // Bierze zapytanie (np. _context.Products) i dodaje np do niego Where(...) z Criteria.
        public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec) //query - np. _context.Products
        {
            if(spec.Criteria != null)
            {
                query = query.Where(spec.Criteria); // x => x.Brand = brand
            }

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescedning != null)
            {
                query = query.OrderByDescending(spec.OrderByDescedning);
            }

            if (spec.IsDistintc)
                query = query.Distinct();

            if(spec.IsPagingEnable) 
                query = query.Skip(spec.Skip).Take(spec.Take);

            //Includes another entitie
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludesString.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }

        public static IQueryable<TResult> GetQuery<TResult>(IQueryable<T> query, ISpecification<T, TResult> spec)
        {
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescedning != null)
                query = query.OrderByDescending(spec.OrderByDescedning);

            if (spec.Select == null)
                throw new InvalidOperationException("Projection (Select) must be provided for this specification.");

            var projectedQuery = query.Select(spec.Select);

            if (spec.IsDistintc)
                projectedQuery = projectedQuery.Distinct();
            
            if(spec.IsPagingEnable)
                projectedQuery = projectedQuery.Skip(spec.Skip).Take(spec.Take);

            return projectedQuery;
        }

    }
}
