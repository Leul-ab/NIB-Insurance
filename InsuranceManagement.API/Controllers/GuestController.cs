using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IPaymentService _paymentService;
        private readonly INewsService _newsService;
        private readonly InsuranceDbContext _context;

        public GuestController(IClientService clientService, IPaymentService paymentService, InsuranceDbContext context, INewsService newsService)
        {
            _clientService = clientService;
            _paymentService = paymentService;
            _context = context;
            _newsService = newsService;
        }

        /// <summary>
        /// Verify life insurance policy by secret key (for beneficiary)
        /// </summary>
        [HttpPost("verify-policy")]
        public async Task<IActionResult> VerifyPolicy([FromForm] VerifyLifePolicyRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.SecretKey) ||
                    string.IsNullOrWhiteSpace(request.BeneficiaryEmail))
                {
                    return BadRequest(new { message = "SecretKey and BeneficiaryEmail are required." });
                }

                var result = await _clientService.VerifyLifePolicyAsync(
                    request.SecretKey,
                    request.BeneficiaryEmail
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        /// <summary>
        /// Report life claim for a verified policy
        /// </summary>
        [HttpPost("report-claim/{applicationId}")]
        public async Task<IActionResult> ReportClaim([FromRoute] Guid applicationId, [FromForm] LifeClaimReportRequest request)
        {
            try
            {
                var claim = await _clientService.ReportLifeClaimAsync(applicationId, request);
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get life claim payout history (guest mode, public)
        /// </summary>
        [HttpGet("life-payout-history")]
        [AllowAnonymous] // Guests can access
        public async Task<IActionResult> GetLifeClaimPayoutHistory([FromQuery] string secretKey, [FromQuery] string beneficiaryPhone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(beneficiaryPhone))
                {
                    return BadRequest(new { message = "Both secretKey and beneficiaryPhone are required." });
                }

                // Get payouts only for this beneficiary under the given policy
                var history = await _paymentService.GetLifeClaimPayoutHistoryAsync(secretKey, beneficiaryPhone);

                if (history == null || !history.Any())
                {
                    return NotFound(new { message = "No payout history found for the provided information." });
                }

                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("news")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNews()
        {
            return Ok(await _newsService.GetPublicNewsAsync());
        }


    }
}
