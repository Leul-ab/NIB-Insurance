using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("initiate/{applicationId}")]
        public async Task<IActionResult> Initiate(Guid applicationId)
        {
            var result = await _paymentService.InitializePaymentAsync(applicationId);
            return Ok(result);
        }

        //[HttpPost("chapa/webhook")]
        //public async Task<IActionResult> Webhook([FromBody] ChapaWebhookRequest request)
        //{
        //    await _paymentService.HandleWebhookAsync(request);
        //    return Ok();
        //}

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string reference)
        {
            var result = await _paymentService.VerifyPaymentAsync(reference);

            return Ok(new
            {
                message = "Payment verification successful",
                data = result
            });
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentTransactions()
        {
            var result = await _paymentService.GetRecentTransactionsAsync();
            return Ok(result);
        }



        


    }

}
