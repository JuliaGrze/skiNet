﻿using CORE.Interfaces;
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

        public int Take {  get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnable {  get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; } = [];

        public List<string> IncludesString { get; } = [];

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

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnable = true;
        }

        public IQueryable<T> ApplyCriteria(IQueryable<T> query)
        {
            if(Criteria != null)
                query = query.Where(Criteria);
            return query;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludesString.Add(includeString);
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
