using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly InsuranceDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public CategoryService(InsuranceDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // ------------------------- IMAGE HANDLING -------------------------

        private async Task<string?> SaveCategoryImageAsync(IFormFile? imageFile)
        {
            // Uploads to Cloudinary — returns a permanent HTTPS URL
            return await _fileStorage.UploadAsync(imageFile, "Categories");
        }

        private void DeleteCategoryImage(string? imageUrl)
        {
            // Files are managed by Cloudinary — no local deletion needed
        }

        // ------------------------- MAIN CATEGORY -------------------------

        public async Task<CategoryResponse> AddMainCategoryAsync(CategoryRequest request)
        {
            // Prevent duplicate names
            if (await _context.OperatorCategories.AnyAsync(c => c.Name == request.Name && c.ParentId == null))
                throw new InvalidOperationException("Main category name already exists.");

            var imageUrl = await SaveCategoryImageAsync(request.ImageFile);

            var mainCategory = new OperatorCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ParentId = null,       // Main category
                FullInsurancePercentage = null,    // Main category has no price
                ThirdPartyPercentage = null,
                HalfLifePrice = null,
                FullLifePrice =null
            };

            _context.OperatorCategories.Add(mainCategory);
            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = mainCategory.Id,
                Name = mainCategory.Name,
                Description = mainCategory.Description,
                ImageUrl = mainCategory.ImageUrl,
                CreatedAt = mainCategory.CreatedAt,
                IsActive = mainCategory.IsActive
            };
        }

        // ------------------------- SUBCATEGORY -------------------------

        public async Task<CategoryResponse> AddSubCategoryAsync(Guid parentCategoryId, CategoryRequest request)
        {
            var parent = await _context.OperatorCategories
                .FirstOrDefaultAsync(c => c.Id == parentCategoryId && c.ParentId == null);

            if (parent == null)
                throw new InvalidOperationException("Parent main category not found.");

            // Prevent duplicate
            if (await _context.OperatorCategories.AnyAsync(c => c.Name == request.Name && c.ParentId == parentCategoryId))
                throw new InvalidOperationException("Subcategory name already exists under this main category.");

            var imageUrl = await SaveCategoryImageAsync(request.ImageFile);

            var subCategory = new OperatorCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                FullInsurancePercentage = request.FullInsurancePercentage,
                ThirdPartyPercentage = request.ThirdPartyPercentage,
                HalfLifePrice = request.HalfLifePrice,
                FullLifePrice = request.FullLifePrice,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ParentId = parentCategoryId
            };

            _context.OperatorCategories.Add(subCategory);
            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                Description = subCategory.Description,
                FullInsurancePercentage = subCategory.FullInsurancePercentage,
                ThirdPartyPercentage = subCategory.ThirdPartyPercentage,
                HalfLifePrice = subCategory.HalfLifePrice,
                FullLifePrice = subCategory.FullLifePrice,
                ImageUrl = subCategory.ImageUrl,
                CreatedAt = subCategory.CreatedAt,
                IsActive = subCategory.IsActive
            };
        }

        // ------------------------- FETCHING -------------------------

        public async Task<IEnumerable<CategoryResponse>> GetAllMainCategoriesAsync()
        {
            var categories = await _context.OperatorCategories
                .Where(c => c.ParentId == null)
                .ToListAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive
            });
        }

        public async Task<IEnumerable<CategoryResponse>> GetSubCategoriesAsync(Guid parentId)
        {
            var subcategories = await _context.OperatorCategories
                .Where(c => c.ParentId == parentId)
                .ToListAsync();

            return subcategories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                FullInsurancePercentage = c.FullInsurancePercentage,
                ThirdPartyPercentage = c.ThirdPartyPercentage,
                HalfLifePrice = c.HalfLifePrice,
                FullLifePrice = c.FullLifePrice,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.OperatorCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return null;

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                FullInsurancePercentage = category.FullInsurancePercentage,
                ThirdPartyPercentage = category.ThirdPartyPercentage,
                HalfLifePrice = category.HalfLifePrice,
                FullLifePrice = category.FullLifePrice,
                CreatedAt = category.CreatedAt,
                IsActive = category.IsActive,
                ParentId = category.ParentId
            };
        }

        // ------------------------- UPDATE CATEGORY -------------------------

        public async Task<bool> UpdateCategoryAsync(Guid id, CategoryRequest request)
        {
            var category = await _context.OperatorCategories.FindAsync(id);
            if (category == null) return false;

            // Check duplicate names
            if (await _context.OperatorCategories.AnyAsync(c =>
                c.Name == request.Name && c.Id != id && c.ParentId == category.ParentId))
            {
                throw new InvalidOperationException("Category name already exists.");
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.IsActive = request.IsActive;

            // Only subcategories can update price
            if (category.ParentId != null)
                category.FullInsurancePercentage = request.FullInsurancePercentage;

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                DeleteCategoryImage(category.ImageUrl);
                category.ImageUrl = await SaveCategoryImageAsync(request.ImageFile);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // ------------------------- DELETE -------------------------

        public async Task<(bool deleted, List<string> warnings)> DeleteCategoryAsync(Guid id, bool forceDelete = false)
        {
            var category = await _context.OperatorCategories
                .Include(c => c.SubCategories)
                .Include(c => c.Operators)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return (false, new List<string>());

            var warnings = new List<string>();

            // MAIN CATEGORY WITH SUBCATEGORIES
            if (category.ParentId == null && category.SubCategories.Any() && !forceDelete)
            {
                warnings.Add("Main category has subcategories.");
                return (false, warnings);
            }

            // SUBCATEGORY ASSIGNED TO OPERATORS
            if (category.Operators.Any() && !forceDelete)
            {
                warnings.Add("Subcategory is assigned to operators.");
                return (false, warnings);
            }

            // Delete subcategories first
            foreach (var sub in category.SubCategories.ToList())
            {
                DeleteCategoryImage(sub.ImageUrl);
                _context.OperatorCategories.Remove(sub);
            }

            // Delete category image
            DeleteCategoryImage(category.ImageUrl);

            _context.OperatorCategories.Remove(category);
            await _context.SaveChangesAsync();

            return (true, warnings);
        }
    }
}
