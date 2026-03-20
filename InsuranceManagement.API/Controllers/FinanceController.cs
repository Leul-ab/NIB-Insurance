using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Finance")]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeService;
        private readonly IPaymentService _paymentService;
        private readonly INewsService _newsService;

        public FinanceController(IFinanceService financeService, IPaymentService paymentService , INewsService newsService)
        {
            _financeService = financeService;
            _paymentService = paymentService;
            _newsService = newsService;
        }



        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            await _financeService.ChangePasswordAsync(userId, request);

            return Ok(new { message = "Password changed successfully." });
        }




        [HttpGet("motor/pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _financeService.GetPendingMotorApplicationsAsync();
            return Ok(result);
        }

        [HttpGet("life/pending")]
        public async Task<IActionResult> GetLifePending()
        {
            var result = await _financeService.GetPendingLifeApplicationsAsync();
            return Ok(result);
        }

        [HttpPost("motor/{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var success = await _financeService.ApproveMotorApplicationAsync(id);
            return success ? Ok("Application approved.") : NotFound();
        }

        [HttpPost("life/{id}/approve")]
        public async Task<IActionResult> ApproveLife(Guid id)
        {
            var success = await _financeService.ApproveLifeApplicationAsync(id);
            return success ? Ok("Application approved.") : NotFound();
        }

        public class RejectRequest
        {
            public string Message { get; set; } = string.Empty;
        }

        [HttpPost("motor/{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequest request)
        {
            var success = await _financeService.RejectMotorApplicationAsync(id, request.Message);
            return success ? Ok("Application rejected.") : NotFound();
        }


        [HttpPost("life/{id}/reject")]
        public async Task<IActionResult> RejectLife(Guid id, [FromBody] RejectRequest request)
        {
            var success = await _financeService.RejectLifeApplicationAsync(id, request.Message);
            return success ? Ok("Application rejected.") : NotFound();
        }


        [HttpGet("motor/approved")]
        public async Task<IActionResult> GetApprovedMotorApplications()
        {
            var result = await _financeService.GetApprovedMotorApplicationsAsync();
            return Ok(result);
        }


        [HttpGet("life/approved")]
        public async Task<IActionResult> GetApprovedLifeApplications()
        {
            var result = await _financeService.GetApprovedLifeApplicationsAsync();
            return Ok(result);
        }

        [HttpGet("motor/rejected")]
        public async Task<IActionResult> GetRejectedMotorApplications()
        {
            var result = await _financeService.GetRejectedMotorApplicationsAsync();
            return Ok(result);
        }

        [HttpGet("life/rejected")]
        public async Task<IActionResult> GetRejectedLifeApplications()
        {
            var result = await _financeService.GetRejectedLifeApplicationsAsync();
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var financeProfile = await _financeService.GetMyProfileAsync(userId);
            if (financeProfile == null)
                return NotFound(new { message = "Client not found." });

            return Ok(financeProfile);
        }

        [HttpGet("finance-dashboard")]
        public async Task<IActionResult> GetFinanceDashboard()
        {
            var dashboard = await _financeService.GetFinanceDashboardAsync();
            return Ok(dashboard);
        }


        /// <summary>
        /// Pays out an approved claim to the client's phone.
        /// </summary>
        /// <param name="claimId">Claim ID to payout</param>
        /// <returns>Payout reference on success</returns>
        [HttpPost("{claimId}/payout")]
        [Authorize(Roles = "Finance,Admin")]
        public async Task<IActionResult> PayoutClaim(Guid claimId)
        {
            try
            {
                var payoutInfo = await _paymentService.PayoutApprovedClaimAsync(claimId);
                return Ok(payoutInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Payout failed",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("approved-claims")]
        public async Task<IActionResult> GetApprovedClaims()
        {
            var claims = await _financeService.GetAllApprovedByManagerClaimsAsync();
            return Ok(claims);
        }


        [HttpGet("payout-history")]
        public async Task<IActionResult> GetPayoutHistory()
        {
            var history = await _paymentService.GetPayoutHistoryAsync();
            return Ok(history);
        }

        [HttpGet("staff-announcements")]
        public async Task<IActionResult> GetOperatorFinanceAnnouncements()
        {
            var announcements = await _newsService.GetOperatorFinanceAnnouncementsAsync();
            return Ok(announcements);
        }


    }
}
