using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Shared;
using System.Linq.Expressions;

namespace ECommerce.Services.Specifications.ProductSpecifications
{
    internal static class ProductSpecificationsHelper
    {
        public static Expression<Func<Product, bool>> GetCriteria(ProductQueryParams queryParams)
        {
            return P =>
                (!queryParams.brandId.HasValue || P.ProductBrandId == queryParams.brandId.Value)
                && (!queryParams.typeId.HasValue || P.ProductTypeId == queryParams.typeId.Value)
                && (
                    string.IsNullOrEmpty(queryParams.Search)
                    || P.Name.ToLower().Contains(queryParams.Search.ToLower())
                );
        }
    }
}
