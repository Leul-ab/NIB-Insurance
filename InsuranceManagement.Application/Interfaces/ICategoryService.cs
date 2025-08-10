using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> AddCategoryAsync(CategoryRequest request);
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(Guid id);
        Task<bool> UpdateCategoryAsync(Guid id, CategoryRequest request);
        Task<(bool deleted, List<string> affectedOperators)> DeleteCategoryAsync(Guid id, bool forceDelete = false);

    }
}
