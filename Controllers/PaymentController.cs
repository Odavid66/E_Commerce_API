using E_commerce_API.DTOs;
using E_commerce_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_API.Controllers
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

        // ── INITIALIZE PAYMENT ─────────────────────────────────────
        [HttpPost("initialize")]
        [Authorize]
        // must be logged in to start a payment 
        public async Task<ActionResult<PaymentResponseDtos>> InitializePayment(IntializePaymentDtos dto)
        // client sends email, amount and orderId in the body
        {
            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero");
            // basic validation — cannot pay zero or negative amount

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required");

            var result = await _paymentService.InitializePayment(dto);

            if (!result.Status)
                return BadRequest(result.Message);
            // Paystack returned an error
            // e.g. invalid email format

            return Ok(result);
        }

        // ── VERIFY PAYMENT ─────────────────────────────────────────
        [HttpGet("verify/{reference}")]
        [Authorize]
        // called after customer completes payment on Paystack page
        public async Task<ActionResult<VerifyPaymentDto>> VerifyPayment(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return BadRequest("Reference is required");

            var result = await _paymentService.VerifyPayment(reference);

            if (!result.Status)
                return BadRequest(result.Message);

            if (result.PaymentStatus != "success")
                return BadRequest($"Payment was not successful. Status: {result.PaymentStatus}");
            // payment was found but not completed
            // e.g. customer abandoned the payment page

            return Ok(result);
        }
    }
}