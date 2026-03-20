using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public class NewsService : INewsService
    {
        private readonly InsuranceDbContext _context;
        private readonly IEmailService _emailService;

        public NewsService(InsuranceDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            string uploadsFolder = Path.Combine("wwwroot", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{uniqueFileName}";
        }

        public async Task<NewsResponse> CreateNewsAsync(Guid managerId, NewsRequest request)
        {
            string? imagePath = null;
            if (request.Image != null)
            {
                imagePath = await SaveFileAsync(request.Image, "NewsImages");
            }

            var news = new News
            {
                Title = request.Title,
                Content = request.Content,
                ImageUrl = imagePath,
                CreatedBy = managerId
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return new NewsResponse
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                ImageUrl = news.ImageUrl,
                CreatedAt = news.CreatedAt
            };
        }

        public async Task<List<NewsResponse>> GetPublicNewsAsync()
        {
            var newsList = await _context.News
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return newsList.Select(n => new NewsResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                ImageUrl = n.ImageUrl,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task<AnnouncementResponse> CreateAnnouncementAsync(Guid managerId, AnnouncementRequest request)
        {
            var announcement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                SystemWide = request.SystemWide,
                CreatedBy = managerId
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return new AnnouncementResponse
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                SystemWide = announcement.SystemWide,
                CreatedAt = announcement.CreatedAt
            };
        }

        public async Task<List<AnnouncementResponse>> GetUserAnnouncementsAsync()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return announcements.Select(a => new AnnouncementResponse
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                SystemWide = a.SystemWide,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task<AnnouncementResponse> CreateOperatorFinanceAnnouncementAsync(Guid createdBy, AnnouncementRequest request)
        {
            var announcement = new Announcement
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                TargetGroup = "OperatorFinance"
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return new AnnouncementResponse
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                CreatedAt = announcement.CreatedAt
            };
        }

        public async Task<List<AnnouncementResponse>> GetOperatorFinanceAnnouncementsAsync()
        {
            return await _context.Announcements
                .Where(a => a.TargetGroup == "OperatorFinance")
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementResponse
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }


    }
}
