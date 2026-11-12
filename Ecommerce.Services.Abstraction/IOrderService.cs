using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string email);
        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetAllDeliveryMethodsAsync();
        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email);
        Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid Id, string email);
    }
}
