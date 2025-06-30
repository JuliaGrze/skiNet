using CORE.Entities;
using CORE.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _storeContext;
        private readonly ConcurrentDictionary<String, object> _repositories = new();
        public UnitOfWork(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task<bool> Complete()
        {
            return await _storeContext.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _storeContext.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;

            //getOrAdd sprawdza czy istnieje klucz type,  jeśli tak – zwraca istniejącą wartość
            // jeśli nie – wywołuje funkcję t => { ... }, zapisuje wynik i zwraca go
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
            {
                //To dynamiczne tworzenie instancji typu generycznego w czasie działania (reflection)

                //MakeGenericType(...) → robi z GenericRepository<> konkretny typ np. GenericRepository<Product>
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
                //Activator.CreateInstance(...) → wywołuje konstruktor new GenericRepository<TEntity>(_storeContext) bez pisania go jawnie
                return Activator.CreateInstance(repositoryType, _storeContext)
                ?? throw new InvalidOperationException($"Could not create repository instance for {t}");
            });

            //lub
            //if (!_repositories.ContainsKey(type))
            //{
            //    var repositoryInstance = new GenericRepository<TEntity>(_storeContext);
            //    _repositories[type] = repositoryInstance;
            //}

            //return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}
