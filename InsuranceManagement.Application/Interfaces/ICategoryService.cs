using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;

namespace InsuranceManagement.Application.Interfaces
{
    public interface ICategoryService
    {
        // -------------------- MAIN CATEGORY --------------------
        Task<CategoryResponse> AddMainCategoryAsync(CategoryRequest request);
        Task<IEnumerable<CategoryResponse>> GetAllMainCategoriesAsync();

        // -------------------- SUBCATEGORY --------------------
        Task<CategoryResponse> AddSubCategoryAsync(Guid parentCategoryId, CategoryRequest request);
        Task<IEnumerable<CategoryResponse>> GetSubCategoriesAsync(Guid parentId);

        // -------------------- COMMON METHODS --------------------
        Task<CategoryResponse?> GetCategoryByIdAsync(Guid id);
        Task<bool> UpdateCategoryAsync(Guid id, CategoryRequest request);

        /// <summary>
        /// Delete category or subcategory.
        /// returns (deleted: bool, warnings: list<string>)
        /// </summary>
        Task<(bool deleted, List<string> warnings)> DeleteCategoryAsync(Guid id, bool forceDelete = false);
    }
}
