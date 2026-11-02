using ECommerce.Shared.DTOs.BasketDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IBasketService
    {
        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO CreateOrUpdateBasket);
        Task<BasketDTO> GetBasketAsync(string basketId);
        Task<bool> DeleteBasketAsync(string basketId);
    }
}
