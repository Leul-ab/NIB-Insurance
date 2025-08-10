using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IOperatorService
    {
        Task<OperatorResponse> AddOperatorAsync(OperatorRequest request);
        Task<IEnumerable<OperatorResponse>> GetAllOperatorsAsync();
        Task<OperatorResponse> GetOperatorByIdAsync(Guid id);
        Task<bool> UpdateOperatorAsync(Guid id, OperatorRequest request);
        Task<bool> DeleteOperatorAsync(Guid id);
        Task<IEnumerable<OperatorResponse>> GetUnassignedOperatorsAsync();
    }
}
