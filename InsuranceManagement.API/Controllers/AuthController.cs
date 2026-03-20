using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IClientService _clientService;

        public AuthController(IAuthService authService, IClientService clientService)
        {
            _authService = authService;
            _clientService = clientService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                //_logger.LogWarning("Login attempt with invalid request data.");
                return BadRequest("Invalid login request.");
            }

            //_logger.LogInformation("Login attempt for email {Email}", request.Email);

            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                //_logger.LogWarning("Unauthorized login attempt for email {Email}", request.Email);
                return Unauthorized(new { message = "Invalid credentials." });
            }

            //_logger.LogInformation("User {Email} logged in successfully", request.Email);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _clientService.SendPasswordResetOtpAsync(request);
            if (!result) return NotFound("User not found.");
            return Ok("OTP sent to your email.");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _clientService.VerifyOtpAsync(request);
            if (!result) return BadRequest("Invalid or expired OTP.");
            return Ok("OTP verified successfully.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _clientService.ResetPasswordAsync(request);
            if (!result) return BadRequest("Cannot reset password. OTP not verified.");
            return Ok("Password reset successfully.");
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest("Email is required.");

            var result = await _clientService.ResendOtpAsync(request);

            if (!result)
                return NotFound("User with this email not found.");

            return Ok("A new OTP has been sent to your email.");
        }

    }
}
