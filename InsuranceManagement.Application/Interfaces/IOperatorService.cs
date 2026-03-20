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
        Task<OperatorResponse> UpdateOperatorAsync(Guid operatorId, OperatorRequest request);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<bool> DeleteOperatorAsync(Guid operatorId);

        Task<IEnumerable<OperatorResponse>> GetUnassignedOperatorsAsync();
        Task<OperatorResponse?> GetMyProfileAsync(Guid userId);
        Task<IEnumerable<ClaimResponse>> GetAllClaimsWithClientInfoAsync();
        Task<List<ClaimResponse>> GetAllLifeClaimsAsync();
        Task<ClaimResponse> GetClaimByIdAsync(Guid claimId);

        Task<ApproveClaimResponse> ReviewClaimAsync(
    Guid operatorUserId,
    Guid claimId,
    ReviewClaimRequest request);

        Task<RejectClaimResponse> RejectClaimAsync(
    Guid operatorUserId,
    Guid claimId,
    RejectClaimRequest request);
        Task<List<ApproveClaimResponse>> GetReviewedClaimsByOperatorAsync(Guid operatorUserId);
       
        Task<List<RejectClaimResponse>> GetAllRejectedClaimsAsync(Guid operatorUserId);
        Task<ApproveClaimResponse> GetReviewedClaimByIdAsync(Guid operatorUserId, Guid claimId);
        Task<List<ClaimResponse>> GetAllReviewedLifeClaimsAsync();
        Task<List<ClaimResponse>> GetAllRejectedLifeClaimsAsync();
        Task<OperatorDashboardResponse> GetOperatorDashboardAsync(Guid operatorUserId);


    }
}
