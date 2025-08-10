using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly InsuranceDbContext _context;

        public OperatorService(InsuranceDbContext context)
        {
            _context = context;
        }
        public async Task<OperatorResponse> AddOperatorAsync(OperatorRequest request)
        {
            var categories = await _context.OperatorCategories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync();

            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName, 
                Role = Domain.Enums.UserRole.Operator,
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.PasswordHash);

            var operatorEntity = new Operator
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                MobilePhone = request.MobilePhone, 
                ImageUrl = request.ImageUrl,
                User = user,
                Categories = categories
            };

            _context.Operators.Add(operatorEntity);
            await _context.SaveChangesAsync();

            return new OperatorResponse
            {
                Id = operatorEntity.Id,
                Email = user.Email,
                ImageUrl = operatorEntity.ImageUrl,
                UserName = user.UserName,
                FullName = operatorEntity.FullName,
                CategoryNames = categories.Select(c => c.Name).ToList(),
                MobilePhone = operatorEntity.MobilePhone
            };
        }

        public async Task<bool> DeleteOperatorAsync(Guid id)
        {
            var op = await _context.Operators.FindAsync(id);
            if (op == null) return false;
            
            _context.Operators.Remove(op);
            await _context.SaveChangesAsync();
            return true;
        }

        public  async Task<IEnumerable<OperatorResponse>> GetAllOperatorsAsync()
        {
            var operators = await _context.Operators
                .Include(o => o.Categories)
                .Include(o => o.User)
                .ToListAsync();

            return operators.Select(op => new OperatorResponse
            {
                Id = op.Id,
                UserName = op.User.UserName,
                Email = op.User.Email,
                FullName = op.FullName,
                MobilePhone = op.MobilePhone,
                ImageUrl = op.ImageUrl,
                CategoryNames = op.Categories.Select(c => c.Name).ToList()
            });
        }

        public async Task<OperatorResponse> GetOperatorByIdAsync(Guid id)
        {
            var op = await _context.Operators
                .Include(o => o.Categories)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if(op == null) return null;

            return new OperatorResponse
            {
                Id = op.Id,
                FullName = op.FullName,
                MobilePhone = op.MobilePhone,
                Email = op.User.Email,
                ImageUrl= op.ImageUrl,
                CategoryNames = op.Categories.Select(c => c.Name).ToList()
            };
        }

       

        public async Task<bool> UpdateOperatorAsync(Guid id, OperatorRequest request)
        {
            var op = await _context.Operators
                .Include(o => o.Categories)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (op == null) return false;

            op.FullName = request.FullName;

            // Replace categories with new ones
            var categories = await _context.OperatorCategories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync();

            op.Categories.Clear();
            foreach (var cat in categories)
            {
                op.Categories.Add(cat);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OperatorResponse>> GetUnassignedOperatorsAsync()
        {
            var unassigned = await _context.Operators
                .Include(o => o.Categories)
                .Where(o => !o.Categories.Any())
                .ToListAsync();

            return unassigned.Select(o => new OperatorResponse
            {
                Id = o.Id,
                UserName = o.User.UserName,
                Email = o.User.Email,
                FullName = o.FullName,
                CategoryNames = new List<string>()
            });
        }
    }
}
