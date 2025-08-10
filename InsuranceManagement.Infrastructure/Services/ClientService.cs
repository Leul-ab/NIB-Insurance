using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly InsuranceDbContext _context;

        public ClientService(InsuranceDbContext context)
        {
            _context = context;
        }
        public async Task<ClientResponse> RegisterClientAsync(ClientRegisterRequest request)
        {
            if (await _context.Clients.AnyAsync(c => c.User.Email == request.Email))
                throw new Exception("Client with this email already exists.");

            var passwordHasher = new PasswordHasher<User>();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                Role = UserRole.Client,
            };

            // Use plain password, not hash here
            user.PasswordHash = passwordHasher.HashPassword(user, request.PasswordHash);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                
                MobliePhone = request.MobilePhone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                UserId = user.Id,
                User = user
            };

            _context.Users.Add(user);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return new ClientResponse
            {
                Id = client.Id,
                FullName = client.FullName,
                Email = client.User.Email,
                MobliePhone = client.MobliePhone,
                CreatedAt = client.CreatedAt
            };
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
