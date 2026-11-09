namespace ECommerce.Shared.DTOs.OrderDTOs
{
    public record OrderToReturnDTO
    {
        public Guid Id { get; init; }

        public string UserEmail { get; init; }

        public ICollection<OrderItemDTO> Items { get; init; }

        public AddressDTO Address { get; init; }

        public string DeliveryMethod { get; init; }

        public string PaymentIntentId { get; set; }
        public string Status { get; init; }

        public DateTimeOffset OrderDate { get; init; }

        public decimal Subtotal { get; init; }

        public decimal Total { get; init; }
    }
}
