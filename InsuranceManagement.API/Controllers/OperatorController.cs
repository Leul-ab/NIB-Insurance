using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Operator")]
    [Route("api/[controller]")]
    public class OperatorController : ControllerBase
    {
        private readonly IOperatorService _operatorService;
        private readonly INewsService _newsService;

        public OperatorController(IOperatorService operatorService, INewsService newsService)
        {
            _operatorService = operatorService;
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

            await _operatorService.ChangePasswordAsync(userId, request);

            return Ok(new { message = "Password changed successfully." });
        }





        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var operatorProfile = await _operatorService.GetMyProfileAsync(userId);
            if (operatorProfile == null)
                return NotFound(new { message = "Client not found." });

            return Ok(operatorProfile);
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetAllClaims()
        {
            try
            {
                var claims = await _operatorService.GetAllClaimsWithClientInfoAsync();

                if (claims == null || !claims.Any())
                    return NotFound(new { message = "No claims found." });

                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("life-claims/pending")]
        public async Task<IActionResult> GetPendingLifeClaims()
        {
            try
            {
                var claims = await _operatorService.GetAllLifeClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}/claims")]
        public async Task<IActionResult> GetClaimById(Guid id)
        {
            try
            {
                var claim = await _operatorService.GetClaimByIdAsync(id);
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }



        [HttpPost("{claimId}/review")]
        public async Task<IActionResult> ReviewClaim(
    Guid claimId,
    [FromBody] ReviewClaimRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var operatorUserId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var result = await _operatorService
                .ReviewClaimAsync(operatorUserId, claimId, request);

            return Ok(result);
        }



        [HttpPost("{claimId}/reject")]
        public async Task<IActionResult> RejectClaim(
    Guid claimId,
    [FromBody] RejectClaimRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var operatorUserId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var result = await _operatorService
                .RejectClaimAsync(operatorUserId, claimId, request);

            return Ok(result);
        }




        [HttpGet("reviewed")]
        public async Task<IActionResult> GetMyReviewedClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var claims = await _operatorService.GetReviewedClaimsByOperatorAsync(userId);
            return Ok(claims);
        }

        [HttpGet("life-claims/reviewed")]
        public async Task<IActionResult> GetReviewedLifeClaims()
        {
            try
            {
                var claims = await _operatorService.GetAllReviewedLifeClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpGet("rejected")]
        public async Task<IActionResult> GetMyRejectedClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var claims = await _operatorService.GetAllRejectedClaimsAsync(userId);
            return Ok(claims);
        }

        [HttpGet("life-claims/rejected")]
        public async Task<IActionResult> GetRejectedLifeClaims()
        {
            try
            {
                var claims = await _operatorService.GetAllRejectedLifeClaimsAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("reviewed/{id}")]
        public async Task<IActionResult> GetReviewedClaimById(Guid id)
        {
            // Safely get userId from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            try
            {
                var claim = await _operatorService.GetReviewedClaimByIdAsync(userId, id);
                return Ok(claim);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var dashboard = await _operatorService.GetOperatorDashboardAsync(userId);

            return Ok(dashboard);
        }

        [HttpGet("staff-announcements")]
        public async Task<IActionResult> GetOperatorFinanceAnnouncements()
        {
            var announcements = await _newsService.GetOperatorFinanceAnnouncementsAsync();
            return Ok(announcements);
        }


    }
}
