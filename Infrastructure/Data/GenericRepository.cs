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
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context;

        public GenericRepository(StoreContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            var query = _context.Set<T>().AsQueryable();

            query = specification.ApplyCriteria(query);

            return await query.CountAsync();    
        }

        public async Task<bool> Exist(int id)
        {
            return await _context.Set<T>().AnyAsync(x => x.Id == id);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetEntityWithSpec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(); 
        }

        public async Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> specification)
        {
            return await ApplySpecification<TResult>(specification).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetListAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsyncWIithSpec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsyncWithSpec<TResult>(ISpecification<T, TResult> specification)
        {
            return await ApplySpecification<TResult>(specification).ToListAsync();
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            //Attach causes the entity to be added to the context (DbContext) in the Unchanged state
            //only observe entity - don't try to add it, just observe it
            _context.Set<T>().Attach(entity);
            //set this entity was updated, generate sql query UPDATE
            _context.Entry(entity).State = EntityState.Modified;
        }

        //ApplySpecification = bierze bazowe zapytanie(_context.Set<T>()) + filtr(spec.Criteria)
        //→ łączy je razem
        //→ i zwraca gotowe zapytanie(IQueryable<T>), które możesz wykonać.
        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);
        }

        private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification)
        {
            return SpecificationEvaluator<T>.GetQuery<TResult>(_context.Set<T>().AsQueryable(), specification);
        }
    }
}
