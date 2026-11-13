using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.BasketDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IPaymentService
    {
        Task<Result<BasketDTO>> CreateOrUpdatePaymentIntentAsync(string basketId);

        Task UpdateOrderPaymentStatus(string request, string stripeSignature);
    }
}
