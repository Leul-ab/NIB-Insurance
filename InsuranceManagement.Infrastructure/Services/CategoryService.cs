using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly InsuranceDbContext _context;

        public CategoryService(InsuranceDbContext context)
        {
            _context = context;
        }
        public async Task<CategoryResponse> AddCategoryAsync(CategoryRequest request)
        {
            string? imageUrl = null;
            if(request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var currentDir = Directory.GetCurrentDirectory();
                var folderPath = Path.Combine(currentDir, "wwwroot", "Uploads", "Categories");
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                imageUrl = Path.Combine("Uploads", "Categories", fileName).Replace("\\", "/");
            }

            var category = new OperatorCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.OperatorCategories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };
        }

        public async Task<(bool deleted, List<string> affectedOperators)> DeleteCategoryAsync(Guid id, bool forceDelete = false)
        {
            var category = await _context.OperatorCategories
                .Include(c => c.Operators)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return (false, new List<string>());

            var assignedOperators = category.Operators.Select(o => o.FullName).ToList();

            if (assignedOperators.Any() && !forceDelete)
            {
                // Warn admin — return operator names but do not delete yet
                return (false, assignedOperators);
            }

            // Unassign operators
            foreach (var op in category.Operators.ToList())
            {
                op.Categories.Remove(category);
            }

            _context.OperatorCategories.Remove(category);
            await _context.SaveChangesAsync();

            return (true, assignedOperators);
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _context.OperatorCategories.ToListAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive
            });
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.OperatorCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, CategoryRequest request)
        {
            var category = await _context.OperatorCategories.FindAsync(id);
            if (category == null) return false;

            category.Name = request.Name;
            category.Description = request.Description;
            category.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
