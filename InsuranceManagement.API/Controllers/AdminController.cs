using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InsuranceManagement.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ICategoryService _categoryService;
        private readonly IOperatorService _operatorService;
        private readonly IFinanceService _financeService;
        private readonly IManagerService _managerService;
        private readonly IWebHostEnvironment _env;
        private readonly IAdminService _adminService;

        public AdminController(
            IClientService clientService,
            ICategoryService categoryService,
            IOperatorService operatorService,
            IFinanceService financeService,
            IManagerService managerService,
            IWebHostEnvironment env,
            IAdminService adminService)
        {
            _clientService = clientService;
            _categoryService = categoryService;
            _operatorService = operatorService;
            _financeService = financeService;
            _managerService = managerService;
            _env = env;
            _adminService = adminService;
        }

        // -------------------------------
        // ✅ OPERATOR CRUD
        // -------------------------------
        [HttpPost("add-operator")]
        public async Task<IActionResult> AddOperator([FromForm] OperatorRequest request)
        {
            

            //request.ImageUrl = imageUrl;
            var result = await _operatorService.AddOperatorAsync(request);
            return Ok(result);
        }

        
        [HttpGet("operators")]
        public async Task<IActionResult> GetAllOperators()
        {
            var result = await _operatorService.GetAllOperatorsAsync();
            return Ok(result);
        }

        [HttpGet("operators/{id}")]
        public async Task<IActionResult> GetOperatorById(Guid id)
        {
            var result = await _operatorService.GetOperatorByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("operator-update{operatorId}")]
        public async Task<IActionResult> UpdateOperator(
    Guid operatorId,
    [FromBody] OperatorRequest request)
        {
            var result = await _operatorService
                .UpdateOperatorAsync(operatorId, request);

            return Ok(result);
        }

        [HttpDelete("operatior-delete{operatorId}")]
        public async Task<IActionResult> DeleteOperator(Guid operatorId)
        {
            await _operatorService.DeleteOperatorAsync(operatorId);
            return Ok(new { message = "Operator deleted successfully." });
        }

        [HttpGet("operators/unassigned")]
        public async Task<IActionResult> GetUnassignedOperators()
        {
            var result = await _operatorService.GetUnassignedOperatorsAsync();
            return Ok(result);
        }

        // -------------------------------
        // ✅ FINANCE CRUD
        // -------------------------------
        [HttpPost("add-finance")]
        public async Task<IActionResult> AddFinance([FromForm] FinanceRequest request, IFormFile? imageFile)
        {
            
            var result = await _financeService.AddFinanceAsync(request);
            return Ok(result);
        }

        [HttpGet("finances")]
        public async Task<IActionResult> GetAllFinances()
        {
            var result = await _financeService.GetAllFinancesAsync();
            return Ok(result);
        }

        [HttpGet("finances/{id}")]
        public async Task<IActionResult> GetFinanceById(Guid id)
        {
            var result = await _financeService.GetFinanceByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("finance-update/{financeId}")]
        public async Task<IActionResult> UpdateFinance(
    Guid financeId,
    [FromBody] FinanceRequest request)
        {
            var result = await _financeService
                .UpdateFinanceAsync(financeId, request);

            return Ok(result);
        }


        [HttpDelete("finance-delete/{financeId}")]
        public async Task<IActionResult> DeleteFinance(Guid financeId)
        {
            await _financeService.DeleteFinanceAsync(financeId);
            return Ok(new { message = "Finance deleted successfully." });
        }

        // -------------------------------
        // ✅ MANAGER CRUD
        // -------------------------------
        [HttpPost("add-manager")]
        public async Task<IActionResult> AddManager([FromForm] ManagerRequest request, IFormFile? imageFile)
        {
            

            //request.ImageUrl = imageUrl;
            var result = await _managerService.AddManagerAsync(request);
            return Ok(result);
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetAllManagers()
        {
            var result = await _managerService.GetAllManagersAsync();
            return Ok(result);
        }

        [HttpGet("managers/{id}")]
        public async Task<IActionResult> GetManagerById(Guid id)
        {
            var result = await _managerService.GetManagerByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("managers-update{id}")]
        public async Task<IActionResult> UpdateManager(
    Guid id,
    ManagerRequest request)
        {
            var result = await _managerService.UpdateManagerAsync(id, request);
            return Ok(result);
        }


        [HttpDelete("managers-delete/{managerId}")]
        public async Task<IActionResult> DeleteManager(Guid managerId)
        {
            await _managerService.DeleteManagerAsync(managerId);
            return Ok(new { message = "Manager deleted successfully." });
        }


        // =================== CLIENT MANAGEMENT ===================
        [HttpPost("add-client")]
        public async Task<IActionResult> RegisterClient([FromForm] ClientRegisterRequest request)
        {
            var result = await _clientService.RegisterClientAsync(request);
            return Ok(result);
        }


        // GET: api/admin/clients
        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(clients);
        }

        // GET: api/admin/clients/{id}
        [HttpGet("clients/{id}")]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null) return NotFound(new { message = "Client not found." });
            return Ok(client);
        }

        // DELETE: api/admin/clients/{id}
        [HttpDelete("clients/{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var deleted = await _clientService.DeleteClientAsync(id);
            if (!deleted) return NotFound(new { message = "Client not found." });
            return NoContent();
        }




        // -------------------------------
        // ✅ CATEGORY CRUD
        // -------------------------------
        [HttpPost("add-maincategory")]
        public async Task<IActionResult> AddMainCategory([FromForm] CategoryRequest request)
        {
            var category = await _categoryService.AddMainCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpGet("get-maincategory")]
        public async Task<IActionResult> GetAllMainCategories()
        {
            var categories = await _categoryService.GetAllMainCategoriesAsync();
            return Ok(categories);
        }

        // -------------------- SUBCATEGORY --------------------

        [HttpPost("{parentId}/add-subcategory")]
        public async Task<IActionResult> AddSubCategory(Guid parentId, [FromForm] CategoryRequest request)
        {
            var subcategory = await _categoryService.AddSubCategoryAsync(parentId, request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = subcategory.Id }, subcategory);
        }

        [HttpGet("{parentId}/get-subcategory")]
        public async Task<IActionResult> GetSubCategories(Guid parentId)
        {
            var subcategories = await _categoryService.GetSubCategoriesAsync(parentId);
            return Ok(subcategories);
        }

        // -------------------- COMMON METHODS --------------------

        [HttpGet("{id}/get-categorybyid")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPut("{id}/update-category")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}/delete-category")]
        public async Task<IActionResult> DeleteCategory(Guid id, [FromQuery] bool force = false)
        {
            var (deleted, warnings) = await _categoryService.DeleteCategoryAsync(id, force);
            if (!deleted) return BadRequest(new { message = "Cannot delete category.", warnings });
            return NoContent();
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("dashboard/user-trends")]
        public async Task<IActionResult> GetUserTrends()
        {
            var result = await _adminService.GetUserTrendsAsync();
            return Ok(result);
        }

    }
}
