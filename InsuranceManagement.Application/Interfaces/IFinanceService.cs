using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IFinanceService
    {
        Task<FinanceResponse> AddFinanceAsync(FinanceRequest request);
        Task<IEnumerable<FinanceResponse>> GetAllFinancesAsync();
        Task<FinanceResponse?> GetFinanceByIdAsync(Guid id);
        Task<FinanceResponse> UpdateFinanceAsync(
    Guid financeId,
    FinanceRequest request);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<bool> DeleteFinanceAsync(Guid financeId);

        Task<List<MotorInsuranceApplicationResponse>> GetPendingMotorApplicationsAsync();
        Task<List<LifeInsuranceApplicationResponse>> GetPendingLifeApplicationsAsync();
        Task<bool> ApproveMotorApplicationAsync(Guid applicationId);
        Task<bool> ApproveLifeApplicationAsync(Guid applicationId);
        Task<bool> RejectMotorApplicationAsync(Guid applicationId, string message);
        Task<bool> RejectLifeApplicationAsync(Guid applicationId, string message);
        Task<List<MotorInsuranceApplicationResponse>> GetApprovedMotorApplicationsAsync();
        Task<List<LifeInsuranceApplicationResponse>> GetApprovedLifeApplicationsAsync();
        Task<List<MotorInsuranceApplicationResponse>> GetRejectedMotorApplicationsAsync();
        Task<List<LifeInsuranceApplicationResponse>> GetRejectedLifeApplicationsAsync();
        Task<FinanceResponse?> GetMyProfileAsync(Guid userId);
        Task<FinanceDashboardResponse> GetFinanceDashboardAsync();

        Task<List<ApproveClaimResponse>> GetAllApprovedByManagerClaimsAsync();

    }
}
