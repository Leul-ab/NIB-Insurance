using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.Interfaces;
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
        private readonly ICategoryService _categoryService;
        private readonly IOperatorService _operatorService;
        private readonly IWebHostEnvironment _env;

        public AdminController(ICategoryService categoryService, IOperatorService operatorService, IWebHostEnvironment env)
        {
            _categoryService = categoryService;
            _operatorService = operatorService;
            _env = env;
        }

        

        [HttpPost("add-operator")]
        public async Task<IActionResult> AddOperator([FromForm] OperatorRequest request, IFormFile? imageFile)
        {

            string? imageUrl = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                imageUrl = Path.Combine("Uploads", "Operators", uniqueFileName).Replace("\\", "/");
            }

            request.ImageUrl = imageUrl;
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

        [HttpPut("operators/{id}")]
        public async Task<IActionResult> UpdateOperator(Guid id, [FromForm] OperatorRequest request)
        {
            var updated = await _operatorService.UpdateOperatorAsync(id, request);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("operators/{id}")]
        public async Task<IActionResult> DeleteOperator(Guid id)
        {
            var deleted = await _operatorService.DeleteOperatorAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("operators/unassigned")]
        public async Task<IActionResult> GetUnassignedOperators()
        {
            var result = await _operatorService.GetUnassignedOperatorsAsync();
            return Ok(result);
        }





        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryRequest request)
        {
            var result = await _categoryService.AddCategoryAsync(request);
            return Ok(result);
        }


        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] CategoryRequest request)
        {
            var updated = await _categoryService.UpdateCategoryAsync(id, request);
            if (!updated) return NotFound();
            return NoContent();
        }

        // First step: warn admin
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id, [FromQuery] bool forceDelete = false)
        {
            var (deleted, affectedOperators) = await _categoryService.DeleteCategoryAsync(id, forceDelete);

            if (!deleted && affectedOperators.Any() && !forceDelete)
            {
                return Conflict(new
                {
                    message = "Category has assigned operators. Use forceDelete=true to remove and unassign them.",
                    affectedOperators
                });
            }

            if (!deleted) return NotFound();

            return Ok(new
            {
                message = "Category deleted successfully.",
                unassignedOperators = affectedOperators
            });
        }
    }
}
