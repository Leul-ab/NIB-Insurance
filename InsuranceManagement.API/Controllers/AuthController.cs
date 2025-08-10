using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
    }
}
