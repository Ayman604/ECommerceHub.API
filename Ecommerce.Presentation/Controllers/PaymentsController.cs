using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.BasketDTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        // POST: baseUrl/api/payments/{basketId}

        [HttpPost("{BasketId}")]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdatePaymentIntent(string BasketId)
        {
            var result = await _paymentService.CreateOrUpdatePaymentIntentAsync(BasketId);
            return HandleResult(result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            await _paymentService.UpdateOrderPaymentStatus(json, stripeSignature!);
            return new EmptyResult();
        }


    }
}
