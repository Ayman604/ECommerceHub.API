using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications.OrderSpecifications;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IMapper mapper,
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork
            )
        {
            _mapper = mapper;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<OrderToReturnDTO>> CreateOrderAsync(
            OrderDTO orderDTO,
            string email
            )
        {
            // 1- Maps the provided shipping address to the order address entity.
            var orderAdress = _mapper.Map<OrderAddress>(orderDTO.ShipToAddress);
            //2- Retrieves the basket and validations its existence.
            var basket = await _basketRepository.GetBasketAsync(orderDTO.BasketId);
            if (basket is null)
            {
                return Error.NotFound("Basket.NotFound", $"Basket with id {orderDTO.BasketId} is not found.");
            }
            if (basket.PaymentIntentId is null)
            {
                return Error.Validation("PaymentIntent.NotFound");
            }

            //3- Creates a list of order items based on the items in the basket.
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (product is null)
                {
                    return Error.NotFound("Product.NotFound", $"Product with id {item.Id} is not found.");
                }
                orderItems.Add(CreateOrderItem(item, product));
            }
            //4- Retrieves the selected delivery method and validates its existence.
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDTO.DeliveryMethodId);
            if (deliveryMethod is null)
            {
                return Error.NotFound("DeliveryMethod.NotFound", $"Delivery method with id {orderDTO.DeliveryMethodId} is not found.");
            }
            //5- Calculates the subtotal of the order by summing up the prices of all order items.
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);
            var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var OrderExistWithThisPaymentIntent = await orderRepo.GetByIdAsync(orderSpec);

            if (OrderExistWithThisPaymentIntent is not null)
            {
                orderRepo.Delete(OrderExistWithThisPaymentIntent);
            }
            //6- Creates a new order entity with the provided details and adds it to the repository.
            var Order = new Order()
            {
                UserEmail = email,
                Address = orderAdress,
                DeliveryMethod = deliveryMethod,
                PaymentIntentId = basket.PaymentIntentId,
                SubTotal = subtotal,
                Items = orderItems,
            };
            await _unitOfWork.GetRepository<Order, Guid>().AddAsync(Order);

            bool result = await _unitOfWork.SaveChangesAsync() > 0;
            if (!result)
            {
                return Error.Faliure("Order.CreationFailed", "Failed to create order.");
            }

            return _mapper.Map<OrderToReturnDTO>(Order);
        }

        public async Task<Result<IEnumerable<DeliveryMethodDTO>>> GetAllDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetAllAsync();

            if (!deliveryMethods.Any())
                return Error.NotFound("DeliveryMethods.NotFound", "No Delivery Methods Found");

            var data = _mapper.Map<IEnumerable<DeliveryMethod>, IEnumerable<DeliveryMethodDTO>>(
                deliveryMethods
            );

            if (data is null)
                return Error.NotFound("DeliveryMethods.NotFound", "No Delivery Methods Found");

            return Result<IEnumerable<DeliveryMethodDTO>>.Ok(data);
        }
        public async Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email)
        {
            var OrderSpec = new OrderSpecification(email);
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(OrderSpec);

            if (!orders.Any())
                return Error.NotFound(
                    "Orders.NotFound",
                    $"No Orders Found for the user with email:{email}"
                );

            var Data = _mapper.Map<IEnumerable<Order>, IEnumerable<OrderToReturnDTO>>(orders);

            return Result<IEnumerable<OrderToReturnDTO>>.Ok(Data);
        }

        public async Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid Id, string email)
        {
            var orderSpec = new OrderSpecification(Id, email);
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderSpec);

            if (order is null)
                return Error.NotFound(
                    "Order.NotFound",
                    $"No Order Found with Id:{Id} for the user with email:{email}"
                );

            var data = _mapper.Map<Order, OrderToReturnDTO>(order);

            return Result<OrderToReturnDTO>.Ok(data);
        }
        private OrderItem CreateOrderItem(BasketItem item, Product product)
        {
            return new OrderItem()
            {
                Product = new ProductItemOrdered()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl,
                },
                Price = product.Price,
                Quantity = item.Quantity,
            };
        }
    }
}
