using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Presistence
{
    internal class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(
            IQueryable<TEntity> entryPoint,
            ISpecifications<TEntity, TKey> specifications
        )
        where TEntity : BaseEntity<TKey>
        {
            var Query = entryPoint; //_dbContext.Orders.Where(o=>o.email==email&&o.Id==id).Include(o=>o.Items).Include(o=>o.DeliveryMethods)

            if (specifications is not null)
            {
                if (specifications.Criteria is not null)
                {
                    Query = Query.Where(specifications.Criteria);
                }

                if (
                    specifications.IncludeExpressions is not null
                    && specifications.IncludeExpressions.Any()
                )
                {
                    Query = specifications.IncludeExpressions.Aggregate(
                        Query,
                        (currentQuery, includeExp) => currentQuery.Include(includeExp)
                    );
                }

                if (specifications.OrderBy is not null)
                {
                    Query = Query.OrderBy(specifications.OrderBy);
                }

                if (specifications.OrderByDescending is not null)
                {
                    Query = Query.OrderByDescending(specifications.OrderByDescending);
                }

                if (specifications.IsPaginated == true)
                {
                    Query = Query.Skip(specifications.Skip).Take(specifications.Take);
                }
            }

            return Query;
        }
    }
}
