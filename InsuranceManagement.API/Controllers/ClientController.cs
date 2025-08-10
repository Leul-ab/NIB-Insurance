using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterClient([FromBody] ClientRegisterRequest request)
        {
            var result = await _clientService.RegisterClientAsync(request);
            return Ok(result);
        }
    }
}
