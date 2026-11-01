using ECommerce.Shared;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.ProductDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductDTO>> GetAllProductsAsync(ProductQueryParams queryParams);

        Task<Result<ProductDTO>> GetProductByIdAsync(int id);

        Task<IEnumerable<BrandDTO>> GetAllBrandsAsync();

        Task<IEnumerable<TypeDTO>> GetAllTypesAsync();
    }
}
