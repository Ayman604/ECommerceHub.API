using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Presistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

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
        public async Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluator
                  .CreateQuery(_dbContext.Set<TEntity>(), specifications)
                  .CountAsync();
        }
        public void Delete(TEntity entity) =>
         _dbContext.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
          => await _dbContext.Set<TEntity>().ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            var Query = SpecificationEvaluator.CreateQuery(_dbContext.Set<TEntity>(),
                specifications);

            return await Query.ToListAsync();
        }
        public async Task<TEntity?> GetByIdAsync(TKey id)
        => await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            var Query = SpecificationEvaluator.CreateQuery(_dbContext.Set<TEntity>(),
                specifications);

            return await Query.FirstOrDefaultAsync();
        }
        public void Update(TEntity entity) =>
            _dbContext.Set<TEntity>().Update(entity);

    }
}
