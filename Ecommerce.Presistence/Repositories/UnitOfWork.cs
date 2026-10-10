using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Presistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbConetxt _dbContext;
        private readonly Dictionary<Type,object> _repositories = [];

        public UnitOfWork(StoreDbConetxt dbContext) 
        {
            _dbContext = dbContext;
        }
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
        {
            // 3 Steps
            var entityType = typeof(TEntity);

            if (_repositories.TryGetValue(entityType, out var repository))
            {
                return (IGenericRepository<TEntity, TKey>)repository;
            }
                

            var newRepo = new GenericRepository<TEntity,TKey>(_dbContext);
            _repositories[entityType]= newRepo; 

            return newRepo;
        }

        public async Task<int> SaveChangesAsync() =>
           await _dbContext.SaveChangesAsync();
    }
}
