using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    /// <summary>
    /// Represents a specification used to filter entities of type <typeparamref name="T"/>.
    /// This pattern allows filtering logic to be encapsulated in an object and reused across the application.
    /// The expression can be translated into SQL by Entity Framework.
    /// </summary>
    /// <typeparam name="T">The entity type to which the specification applies.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Gets the filtering criteria as a LINQ expression.
        /// This expression is used by Entity Framework to generate SQL queries.
        /// </summary>
        //Func - delegat, function
        //Expression - for Entity Framework, it translete into sql
        Expression<Func<T, bool>>? Criteria { get; }


        /// <summary>
        /// Gets the expression used to specify ascending sorting for the result set.
        /// This expression is typically translated by Entity Framework into an ORDER BY clause in SQL.
        /// </summary>
        Expression<Func<T, object>>? OrderBy { get; }

        /// <summary>
        /// Gets the expression used to specify descending sorting for the result set.
        /// This expression is typically translated by Entity Framework into an ORDER BY ... DESC clause in SQL.
        /// </summary>
        Expression<Func<T, object>>? OrderByDescedning { get; }

        /// <summary>
        /// stores uniqueness
        /// </summary>
        bool IsDistintc {  get; }


    }
    // filter, sorting,... + add Projection = returrn ProductDto instead Product
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>>? Select { get; }
    }
}
