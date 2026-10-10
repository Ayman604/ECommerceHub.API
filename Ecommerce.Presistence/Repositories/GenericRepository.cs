using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Presistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presistence.Repositories
{
    public class GenericRepository<TEntity, TKey>
        : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbConetxt _dbContext;
        public GenericRepository(StoreDbConetxt dbConetxt)
        {
            _dbContext = dbConetxt;
        }
        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity) =>
         _dbContext.Set<TEntity>().Remove(entity);
        
        public async Task<IEnumerable<TEntity>> GetAllAsync()
          => await  _dbContext.Set<TEntity>().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id)
        => await _dbContext.Set<TEntity>().FindAsync(id);
        public void Update(TEntity entity)=>
            _dbContext.Set<TEntity>().Update(entity);
        
    }
}
