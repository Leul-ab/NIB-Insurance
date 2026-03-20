using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.DTOs.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IPaymentService _paymentService;
        private readonly INewsService _newsService;


        public ClientController(IClientService clientService, IPaymentService paymentService, INewsService newsService)
        {
            _clientService = clientService;
            _paymentService = paymentService;
            _newsService = newsService;
        }

        // --------------------------
        // REGISTER CLIENT
        // --------------------------
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterClient([FromForm] ClientRegisterRequest request)
        {
            try
            {
                var result = await _clientService.RegisterClientAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --------------------------
        // UPDATE CLIENT PROFILE
        // --------------------------
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateClient([FromForm] ClientUpdateRequest request)
        {
            try
            {
                var client = await GetLoggedInClientAsync();
                var updatedClient = await _clientService.UpdateClientAsync(client.Id, request);
                return Ok(updatedClient);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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

            await _clientService.ChangePasswordAsync(userId, request);

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

            var clientProfile = await _clientService.GetMyProfileAsync(userId);
            if (clientProfile == null)
                return NotFound(new { message = "Client not found." });

            return Ok(clientProfile);
        }


        [HttpPost("apply/motor/preview")]
        public async Task<IActionResult> Preview([FromForm] MotorInsuranceApplyRequest request)
        {
            var client = await GetLoggedInClientAsync();
            var result = await _clientService.PreviewMotorInsuranceAsync(client.Id, request);
            return Ok(result);
        }

        



        [HttpPost("apply/motor/confirm")]
        public async Task<IActionResult> Confirm([FromForm] MotorInsuranceConfirmRequest request)
        {
            var client = await GetLoggedInClientAsync();
            var result = await _clientService.ConfirmMotorInsuranceAsync(client.Id, request);
            return Ok(result);
        }




        [HttpPost("apply/life/preview")]
        public async Task<IActionResult> PreviewLife([FromForm] LifeInsuranceApplyRequest request)
        {
            var client = await GetLoggedInClientAsync();
            var result = await _clientService.PreviewLifeInsuranceAsync(client.Id, request);
            return Ok(result);
        }

        [HttpPost("apply/life/{applicationId}/beneficiary")]
        public async Task<IActionResult> AddBeneficiary(
    Guid applicationId,
    [FromForm] BeneficiaryRequest beneficiary)
        {
            try
            {
                var addedBeneficiary = await _clientService.AddSingleBeneficiaryLifeInsuranceAsync(applicationId, beneficiary);
                return Ok(addedBeneficiary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }




        [HttpPost("apply/life/confirm")]
        public async Task<IActionResult> ConfirmLife([FromBody] LifeInsuranceConfirmRequest request)
        {
            var client = await GetLoggedInClientAsync();
            var result = await _clientService.ConfirmLifeInsuranceAsync(client.Id, request);
            return Ok(result);
        }







        // --------------------------
        // GET ALL MOTOR INSURANCE APPLICATIONS
        // --------------------------
        [HttpGet("applications/motor")]
        public async Task<IActionResult> GetMotorApplications()
        {
            try
            {
                var client = await GetLoggedInClientAsync();
                var result = await _clientService.GetClientMotorApplicationsAsync(client.Id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("applications/life")]
        public async Task<IActionResult> GetLifeApplications()
        {
            try
            {
                var client = await GetLoggedInClientAsync();
                var result = await _clientService.GetClientLifeApplicationsAsync(client.Id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        

        [HttpGet("transactions/me")]
        public async Task<IActionResult> GetMyTransactions()
        {
            // 1. Read the user ID from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            // 2. Convert userId → clientId
            var client = await _clientService.GetMyProfileAsync(userId);
            if (client == null)
                return NotFound(new { message = "Client not found." });

            var clientId = client.Id;

            // 3. Fetch only this client's transactions
            var transactions = await _paymentService.RecentTransactionsByUserAsync(clientId);

            if (!transactions.Any())
                return NotFound(new { message = "No transactions found for this user." });

            return Ok(transactions);
        }


        [HttpGet("applications/Motorpaid")]
        public async Task<IActionResult> GetPaidApplications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var paidApps = await _paymentService.PaidApplicationsByUserAsync(userId);

            if (paidApps == null || !paidApps.Any())
                return NotFound(new { message = "No paid applications found for this user." });

            return Ok(paidApps);
        }


        [HttpGet("applications/Lifepaid")]
        public async Task<IActionResult> GetPaidLifeApplications()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var paidApps = await _paymentService.PaidLifeApplicationsByUserAsync(userId);

            if (paidApps == null || !paidApps.Any())
                return NotFound(new { message = "No paid applications found for this user." });

            return Ok(paidApps);
        }

        [HttpPost("reveal-secret")]
        public async Task<IActionResult> RevealSecret([FromBody] RevealSecretKeyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var response = await _paymentService.RevealSecretKeyAsync(userId, request);
            return Ok(response);
        }





        // --------------------------
        // HELPER: Get logged-in client
        // --------------------------
        private async Task<ClientResponse> GetLoggedInClientAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User is not logged in.");

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID in token.");

            var client = await _clientService.GetClientByUserIdAsync(userId);
            if (client == null)
                throw new UnauthorizedAccessException("Client not found.");

            return client;
        }


        [HttpGet("client-dashboard")]
        public async Task<IActionResult> GetClientDashboard()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            // Get client using UserId (you already have this service method)
            var client = await _clientService.GetClientByUserIdAsync(userId);
            if (client == null)
                return NotFound(new { message = "Client profile not found." });

            var dashboard = await _clientService.GetClientDashboardAsync(client.Id);

            return Ok(dashboard);
        }


        [HttpPost("claims/motor/{motorApplicationId}")]
        public async Task<IActionResult> ReportMotorClaim(
    Guid motorApplicationId,
    [FromForm] MotorClaimReportRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            try
            {
                var response = await _clientService.ReportMotorClaimAsync(userId, motorApplicationId, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    //    [HttpPost("claims/life/{lifeApplicationId}")]
    //    public async Task<IActionResult> ReportLifeClaim(
    //Guid lifeApplicationId,
    //[FromForm] LifeClaimReportRequest request)
    //    {
    //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //        if (string.IsNullOrEmpty(userIdClaim))
    //            return Unauthorized(new { message = "User is not logged in." });

    //        if (!Guid.TryParse(userIdClaim, out var userId))
    //            return BadRequest(new { message = "Invalid user ID in token." });

    //        try
    //        {
    //            var response = await _clientService.ReportLifeClaimAsync(userId, lifeApplicationId, request);
    //            return Ok(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(new { message = ex.Message });
    //        }
    //    }

        [HttpGet("claims")]
        public async Task<IActionResult> GetMyClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            var claims = await _clientService.GetMyClaimsAsync(userId);

            if (claims == null || !claims.Any())
                return NotFound(new { message = "No claims found for this user." });

            return Ok(claims);
        }

        [HttpGet("my-claims/by-status")]
        public async Task<IActionResult> GetMyClaimsByStatus()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });
            var result = await _clientService.GetMyClaimsByStatusAsync(userId);
            return Ok(result);
        }



        [HttpGet("my-payouts")]
        public async Task<IActionResult> GetMyPayoutHistory()
        {
            // Get logged-in user ID from JWT
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            // Call service to get payouts for this client
            var payouts = await _clientService.GetClientPayoutHistoryAsync(userId);

            if (payouts == null || !payouts.Any())
                return NotFound(new { message = "No payout history found for this user." });

            return Ok(payouts);
        }

        [HttpGet("announcements")]
        [Authorize] // requires logged in user
        public async Task<IActionResult> GetAnnouncements()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "User is not logged in." });

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest(new { message = "Invalid user ID in token." });

            return Ok(await _newsService.GetUserAnnouncementsAsync());
        }

    }
}
