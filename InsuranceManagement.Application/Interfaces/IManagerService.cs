using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IManagerService
    {
        Task<ManagerResponse> AddManagerAsync(ManagerRequest request);
        Task<IEnumerable<ManagerResponse>> GetAllManagersAsync();
        Task<ManagerResponse?> GetManagerByIdAsync(Guid id);
        Task<ManagerResponse> UpdateManagerAsync(
    Guid managerId,
    ManagerRequest request);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<bool> DeleteManagerAsync(Guid id);
        Task<ManagerResponse?> GetMyProfileAsync(Guid userId);
        Task<ApproveClaimResponse> ApproveReviewedClaimAsync(
    Guid managerUserId,
    Guid claimId);

        Task<RejectClaimResponse> RejectReviewedClaimAsync(
    Guid managerUserId,
    Guid claimId,
    ManagerRejectClaimRequest request);

        Task<List<ReviewedClaimDto>> GetClaimsReviewedByOperatorsAsync();
        Task<List<ClaimResponse>> GetAllReviewedByOperatorLifeClaimsAsync();


        Task<List<ReviewedClaimDto>> GetApprovedReviewedClaimsAsync();
        Task<List<ReviewedClaimDto>> GetRejectedReviwedClaimsAsync();
        Task<List<ClaimResponse>> GetAllLifeApprovedByManagerAsync();
        Task<List<ClaimResponse>> GetAllLifeRejectedByManagerAsync();
        Task<ReviewedClaimDto> GetReviewedClaimByIdAsync(Guid claimId);
        Task<ClaimResponse> GetReviewedLifeClaimByIdAsync(Guid claimId);
        Task<List<ClaimPayoutManagerResponse>> GetAllPayoutsForManagerAsync();
        Task<ManagerDashboardStatsDto> GetManagerDashboardStatsAsync();
    }
}
