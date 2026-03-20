using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface INewsService
    {
        Task<NewsResponse> CreateNewsAsync(Guid managerId, NewsRequest request);
        Task<List<NewsResponse>> GetPublicNewsAsync();
        Task<AnnouncementResponse> CreateAnnouncementAsync(Guid managerId, AnnouncementRequest request);
        Task<List<AnnouncementResponse>> GetUserAnnouncementsAsync();

        Task<AnnouncementResponse> CreateOperatorFinanceAnnouncementAsync(Guid createdBy, AnnouncementRequest request);
        Task<List<AnnouncementResponse>> GetOperatorFinanceAnnouncementsAsync();


    }
}
