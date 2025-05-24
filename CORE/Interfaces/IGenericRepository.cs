using CORE.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    /// <summary>
    /// Generic repository interface for performing CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">The entity type, which must inherit from BaseEntity.</typeparam>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a list of all entities.
        /// </summary>
        /// <returns>A read-only list of entities.</returns>
        Task<IReadOnlyList<T>> GetListAsync();

        /// <summary>
        /// Retrieves a single entity that matches the specified specification criteria.
        /// </summary>
        /// <param name="specification">The specification that defines the filtering logic.</param>
        /// <returns>The entity that matches the specification; otherwise, null if none found.</returns>
        Task<T?> GetEntityWithSpec(ISpecification<T> specification);

        /// <summary>
        /// Retrieves a list of entities that match the specified specification criteria.
        /// </summary>
        /// <param name="specification">The specification that defines the filtering logic.</param>
        /// <returns>A read-only list of entities that match the specification.</returns>
        Task<IReadOnlyList<T>> ListAsyncWIithSpec(ISpecification<T> specification);

        /// <summary>
        /// Retrieves a single projected result that matches the specified specification.
        /// This method applies filtering, sorting, and projection logic defined in the specification,
        /// and returns the first matching element as a projected <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the projection (e.g., a DTO).</typeparam>
        /// <param name="specification">The specification that defines filtering, sorting, and projection logic.</param>
        /// <returns>The projected result if found; otherwise, null.</returns>
        Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> specification);


        /// <summary>
        /// Retrieves a list of projected results that match the specified specification.
        /// This method applies filtering, sorting, and projection logic defined in the specification,
        /// and returns a collection of <typeparamref name="TResult"/> items.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the projection (e.g., a DTO).</typeparam>
        /// <param name="specification">The specification that defines filtering, sorting, and projection logic.</param>
        /// <returns>A read-only list of projected results.</returns>
        Task<IReadOnlyList<TResult>> ListAsyncWithSpec<TResult>(ISpecification<T, TResult> specification);
    


        /// <summary>
        /// Adds a new entity to the context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        /// <summary>
        /// Updates an existing entity in the context.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity from the context.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Saves all changes made in the context.
        /// </summary>
        /// <returns>True if at least one change was saved; otherwise, false.</returns>
        Task<bool> SaveAllAsync();

        /// <summary>
        /// Checks whether an entity with the given ID exists.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>True if the entity exists; otherwise, false.</returns>
        Task<bool> Exist(int id);
    }
}
