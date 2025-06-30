using CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Interfaces
{
    /// <summary>
    /// Represents a unit of work that coordinates the work of multiple repositories 
    /// and ensures that all changes are committed as a single transaction.
    /// </summary>
    //IDisposable to interfejs, który pozwala zwalniać zasoby (np. pamięć, połączenia do bazy, pliki, strumienie) ręcznie
    //– zamiast czekać, aż zrobi to Garbage Collector.
    public interface IUnitOfWork : IDisposable
    {
        //To typ zwracany – czyli metoda Repository<TEntity>() zwraca instancję repozytorium dla danego typu encji
        // np. uzycie - var repo = unitOfWork.Repository<Product>(); 

        /// <summary>
        /// Returns a generic repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that inherits from BaseEntity.</typeparam>
        /// <returns>An instance of IGenericRepository for the given entity type.</returns>
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Saves all changes made in the current unit of work.
        /// </summary>
        /// <returns>True if at least one change was saved successfully; otherwise, false.</returns>
        Task<bool> Complete();
    }
}
