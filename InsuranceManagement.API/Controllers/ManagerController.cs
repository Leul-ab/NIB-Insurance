using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Manager")]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly INewsService _newsService;

        public ManagerController(IManagerService managerService, INewsService newsService)
        {
            _managerService = managerService;
            _newsService = newsService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var managerProfile = await _managerService.GetMyProfileAsync(userId);
            if (managerProfile == null)
                return NotFound(new { message = "Client not found." });

            return Ok(managerProfile);
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

            await _managerService.ChangePasswordAsync(userId, request);

            return Ok(new { message = "Password changed successfully." });
        }



        /// <summary>
        /// Manager approves or rejects a claim that has been reviewed by an operator.
        /// </summary>
        /// <param name="claimId">The ID of the claim to approve or reject</param>
        /// <param name="request">Approval request (approve or reject)</param>
        /// <returns>Approved claim info</returns>
        [HttpPost("{claimId}/approve")]
        public async Task<IActionResult> ApproveReviewedClaim(Guid claimId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var managerUserId = Guid.Parse(userIdClaim);

            var result = await _managerService
                .ApproveReviewedClaimAsync(managerUserId, claimId);

            return Ok(result);
        }





        
        [HttpPost("{claimId}/reject")]
        public async Task<IActionResult> RejectReviewedClaim(
    Guid claimId,
    [FromBody] ManagerRejectClaimRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var managerUserId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var result = await _managerService
                .RejectReviewedClaimAsync(managerUserId, claimId, request);

            return Ok(result);
        }





        [HttpGet("reviwed-pending")]
        public async Task<IActionResult> GetPendingReviewedClaims()
        {
            try
            {
                var claims = await _managerService.GetClaimsReviewedByOperatorsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("life-claims/reviewed-operator")]
        public async Task<IActionResult> GetReviewedByOperator()
        {
            try
            {
                var claims = await _managerService.GetAllReviewedByOperatorLifeClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // GET: api/manager/claims/approved
        [HttpGet("manager-approved")]
        public async Task<IActionResult> GetAllApprovedReviewedClaims()
        {
            try
            {
                var claims = await _managerService.GetApprovedReviewedClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("manager-rejected")]
        public async Task<IActionResult> GetAllRejectedReviewedClaims()
        {
            try
            {
                var claims = await _managerService.GetRejectedReviwedClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("life-claims/approved")]
        public async Task<IActionResult> GetApprovedClaims()
        {
            try
            {
                var claims = await _managerService.GetAllLifeApprovedByManagerAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("life-claims/rejected")]
        public async Task<IActionResult> GetRejectedClaims()
        {
            try
            {
                var claims = await _managerService.GetAllLifeRejectedByManagerAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/manager/claims/reviewed/{id}
        [HttpGet("reviewed/{id}")]
        public async Task<IActionResult> GetReviewedClaimById(Guid id)
        {
            try
            {
                var claim = await _managerService.GetReviewedClaimByIdAsync(id);
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        //life claims
        [HttpGet("life-claims/reviewed-operator/{claimId}")]
        public async Task<IActionResult> GetReviewedLifeClaimById(Guid claimId)
        {
            try
            {
                var claim = await _managerService.GetReviewedLifeClaimByIdAsync(claimId);
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("all-payouts")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetAllPayouts()
        {
            var payouts = await _managerService.GetAllPayoutsForManagerAsync();

            if (payouts == null || !payouts.Any())
                return NotFound(new { message = "No payouts found." });

            return Ok(payouts);
        }

        


        [HttpPost("news")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PostNews([FromForm] NewsRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User not logged in or invalid token" });

            var managerId = Guid.Parse(userIdClaim);

            return Ok(await _newsService.CreateNewsAsync(managerId, request));
        }

        [HttpPost("staff-announcements")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PostOperatorAndFinanceAnnouncement([FromBody] AnnouncementRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User not logged in." });

            var managerId = Guid.Parse(userIdClaim);

            return Ok(await _newsService.CreateOperatorFinanceAnnouncementAsync(managerId, request));
        }


        [HttpPost("announcements")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PostAnnouncement([FromBody] AnnouncementRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User not logged in or invalid token" });

            var managerId = Guid.Parse(userIdClaim);

            return Ok(await _newsService.CreateAnnouncementAsync(managerId, request));
        }

        [HttpGet("staff-announcements")]
        
        public async Task<IActionResult> GetOperatorFinanceAnnouncements()
        {
            var announcements = await _newsService.GetOperatorFinanceAnnouncementsAsync();
            return Ok(announcements);
        }


        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _managerService.GetManagerDashboardStatsAsync();
            return Ok(stats);
        }

    }
}
