using CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        private readonly Expression<Func<T, bool>>? _criteria;
        public BaseSpecification(Expression<Func<T, bool>>? criteria)
        {
            _criteria = criteria;
        }

        protected BaseSpecification() : this(null) { } //Allows you to create inheriting classes without having to provide Criteria

        // Przechowuje Criteria – warunek typu Expression<Func<T, bool>>
        // (np.product => product.IsActive && product.Price > 100)
        public Expression<Func<T, bool>>? Criteria => _criteria; //readonly

        public Expression<Func<T, object>>? OrderBy {  get; private set; }

        public Expression<Func<T, object>>? OrderByDescedning {  get; private set; }

        public bool IsDistintc {  get; private set; }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByExpression)
        {
            OrderByDescedning = orderByExpression;
        }

        /// <summary>
        /// Set true if query result is unique
        /// </summary>
        protected void AddDistinct()
        {
            IsDistintc = true;
        }

    }

    public class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
    {
        protected BaseSpecification() : base() { }
        //transform Product -> ProductDto
        public Expression<Func<T, TResult>>? Select {  get; private set; }

        protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
            { Select = selectExpression; }
    }
}
