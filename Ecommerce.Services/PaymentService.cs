using AutoMapper;
using ECommerce.Domain.Contracts;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Services
{
    internal class PaymentService //: IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PaymentService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper
        )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        //public async Task<Result<BasketDTO>> CreateOrUpdatePaymentIntentAsync(string basketId)
        //{
        //    var skey = _configuration["Stripe:skey"];
        //    if (skey is null)
        //        return Error.Failure("Failed to obtain Secret Key Value");
        //    StripeConfiguration.ApiKey = skey;
        //    // 1-Retrieve the basket by its ID
        //    var basket = await _basketRepository.GetBasketAsync(basketId);
        //    if (basket is null)
        //        return Error.NotFound("Basket not found");

        //    //2-Validate Delivery Method Inside Basket

        //    if (basket.DeliveryMethodId is null) // 9
        //        return Error.Validation("Delivery method is not selected in the basket");

        //    //3-Retrieve the delivery method details from the database

        //    var method = await _unitOfWork
        //        .GetRepository<DeliveryMethod, int>()
        //        .GetByIdAsync(basket.DeliveryMethodId.Value);

        //    if (method is null)
        //        return Error.NotFound("Delivery method not found");
        //    basket.ShippingPrice = method.Price;

        //    foreach (var item in basket.Items)
        //    {
        //        var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);

        //        if (product is null)
        //            return Error.NotFound("ProductItem.NotFound");

        //        item.Price = product.Price;
        //        item.ProductName = product.Name;
        //        item.PictureUrl = product.PictureUrl;
        //    }

        //    long amount = (long)(basket.Items.Sum(I => I.Quantity * I.Price) * 100); //1 dollar=>100 cents

        //    //4-Create or update payment intent with Stripe API

        //    //basket PaymentIntentId is null =>Create

        //    var stripeservice = new PaymentIntentService();
        //    if (basket.PaymentIntentID is null) //Create
        //    {
        //        #region Integration with External Service
        //        //Integration with any external service
        //        //Download Stripe Nuget Package
        //        // Collection of classes as DLL
        //        // Main class to interact with stripe API [Create from it object]
        //        // Use service inside the main Object [Call function]
        //        #endregion

        //        var options = new PaymentIntentCreateOptions
        //        {
        //            Amount = amount,
        //            Currency = "USD",
        //            PaymentMethodTypes = ["card"],
        //        };
        //        var paymentIntent = await stripeservice.CreateAsync(options); //External API Call

        //        basket.PaymentIntentID = paymentIntent.Id;
        //        basket.ClientSecret = paymentIntent.ClientSecret;
        //    }
        //    //basket PaymentIntentId is not null =>Update
        //    else
        //    {
        //        var options = new PaymentIntentUpdateOptions { Amount = amount };

        //        await stripeservice.UpdateAsync(basket.PaymentIntentID, options);
        //    }

        //    await _basketRepository.CreateOrUpdateBasketAsync(basket);

        //    return _mapper.Map<BasketDTO>(basket);
        //}
        //public async Task UpdateOrderPaymentStatus(string request, string stripeSignature)
        //{
        //    var endPointSecret = _configuration["Stripe:EndPointSecret"];
        //    var stripeEvent = EventUtility.ConstructEvent(request, stripeSignature, endPointSecret);

        //    // Handle the event
        //    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        //    var order = await _unitOfWork
        //        .GetRepository<Order, Guid>()
        //        .GetByIdAsync(new OrderWithPaymentIntentSpecifications(paymentIntent!.Id));
        //    if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
        //    {
        //        order.Status = OrderStatus.PaymentReceived;

        //        _unitOfWork.GetRepository<Order, Guid>().Update(order);

        //        await _unitOfWork.SaveChangesAsync();
        //    }
        //    else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
        //    {
        //        order.Status = OrderStatus.PaymentFailed;
        //        _unitOfWork.GetRepository<Order, Guid>().Update(order);
        //        await _unitOfWork.SaveChangesAsync();
        //    }
        //    // ... handle other event types
        //    else
        //    {
        //        Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
        //    }
        //}
    }
}
