using ECommerce.Domain.Entities.OrderModule;

namespace ECommerce.Services.Specifications.OrderSpecifications
{
    internal class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order, Guid>
    {
        public OrderWithPaymentIntentSpecifications(string paymentIntentId)
            : base(O => O.PaymentIntentId == paymentIntentId) { }

    }
}
