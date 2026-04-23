using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InsuranceManagement.Infrastructure.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly InsuranceDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _fileStorage;

        public OperatorService(InsuranceDbContext context, IEmailService emailService, IFileStorageService fileStorage)
        {
            _context = context;
            _emailService = emailService;
            _fileStorage = fileStorage;
        }

        public async Task<OperatorResponse> AddOperatorAsync(OperatorRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email is already registered.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var categories = await _context.OperatorCategories
                    .Where(c => request.CategoryIds.Contains(c.Id))
                    .ToListAsync();

                var passwordHasher = new PasswordHasher<User>();

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    UserName = request.Email,
                    Role = UserRole.Operator,
                    PasswordHash = passwordHasher.HashPassword(null, request.Password) // ✅ hash the actual password
                };

                // Upload profile image to Cloudinary — returns permanent HTTPS URL
                var logoImageUrl = await _fileStorage.UploadAsync(request.LogoImageUrl, "operators");

                var operatorEntity = new Operator
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    FatherName = request.FatherName,
                    GrandFatherName = request.GrandFatherName,
                    LogoImageUrl = logoImageUrl,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber,
                    Region = request.Region,
                    City = request.City,
                    SubCity = request.SubCity,
                    NationalIdOrPassport = request.NationalIdOrPassport,
                    CreatedAt = request.CreatedAt == default ? DateTime.UtcNow : request.CreatedAt,
                    User = user,
                    Categories = categories
                };

                _context.Operators.Add(operatorEntity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OperatorResponse
                {
                    Id = operatorEntity.Id,
                    FirstName = operatorEntity.FirstName,
                    FatherName = operatorEntity.FatherName,
                    GrandFatherName = operatorEntity.GrandFatherName,
                    LogoImageUrl =operatorEntity.LogoImageUrl,
                    Email = user.Email,
                    DateOfBirth = operatorEntity.DateOfBirth,
                    Gender = (Gender)operatorEntity.Gender,
                    PhoneNumber = operatorEntity.PhoneNumber,
                    Region = operatorEntity.Region,
                    City = operatorEntity.City,
                    SubCity = operatorEntity.SubCity,
                    NationalIdOrPassport = operatorEntity.NationalIdOrPassport,
                    CreatedAt = operatorEntity.CreatedAt,
                    CategoryNames = categories.Select(c => c.Name).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<OperatorResponse>> GetAllOperatorsAsync()
        {
            var operators = await _context.Operators
                .Include(o => o.User)
                .Include(o => o.Categories)
                .ToListAsync();

            return operators.Select(op => new OperatorResponse
            {
                Id = op.Id,
                FirstName = op.FirstName,
                FatherName = op.FatherName,
                GrandFatherName = op.GrandFatherName,
                Email = op.User.Email,
                DateOfBirth = op.DateOfBirth,
                Gender = (Gender)op.Gender,
                PhoneNumber = op.PhoneNumber,
                Region = op.Region,
                City = op.City,
                SubCity = op.SubCity,
                NationalIdOrPassport = op.NationalIdOrPassport,
                CreatedAt = op.CreatedAt,
                CategoryNames = op.Categories.Select(c => c.Name).ToList()
            });
        }
        public async Task<OperatorResponse?> GetOperatorByIdAsync(Guid id)
        {
            var op = await _context.Operators
                .Include(o => o.User)
                .Include(o => o.Categories)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (op == null) return null;

            return new OperatorResponse
            {
                Id = op.Id,
                FirstName = op.FirstName,
                FatherName = op.FatherName,
                GrandFatherName = op.GrandFatherName,
                Email = op.User.Email,
                DateOfBirth = op.DateOfBirth,
                Gender = (Gender)op.Gender,
                PhoneNumber = op.PhoneNumber,
                Region = op.Region,
                City = op.City,
                SubCity = op.SubCity,
                NationalIdOrPassport = op.NationalIdOrPassport,
                CreatedAt = op.CreatedAt,
                CategoryNames = op.Categories.Select(c => c.Name).ToList()
            };
        }
        public async Task<OperatorResponse> UpdateOperatorAsync(Guid operatorId, OperatorRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var operatorEntity = await _context.Operators
                    .Include(o => o.User)
                    .Include(o => o.Categories)
                    .FirstOrDefaultAsync(o => o.Id == operatorId);

                if (operatorEntity == null)
                    throw new Exception("Operator not found.");

                // Prevent duplicate email
                if (operatorEntity.User.Email != request.Email &&
                    await _context.Users.AnyAsync(u => u.Email == request.Email))
                    throw new InvalidOperationException("Email is already registered.");

                // Load categories
                var categories = await _context.OperatorCategories
                    .Where(c => request.CategoryIds.Contains(c.Id))
                    .ToListAsync();

                // Update User
                operatorEntity.User.Email = request.Email;
                operatorEntity.User.UserName = request.Email;

                // Optional password update
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    var passwordHasher = new PasswordHasher<User>();
                    operatorEntity.User.PasswordHash =
                        passwordHasher.HashPassword(operatorEntity.User, request.Password);
                }

                // Update Operator info
                operatorEntity.FirstName = request.FirstName;
                operatorEntity.FatherName = request.FatherName;
                operatorEntity.GrandFatherName = request.GrandFatherName;
                operatorEntity.DateOfBirth = request.DateOfBirth;
                operatorEntity.Gender = request.Gender;
                operatorEntity.PhoneNumber = request.PhoneNumber;
                operatorEntity.Region = request.Region;
                operatorEntity.City = request.City;
                operatorEntity.SubCity = request.SubCity;
                operatorEntity.NationalIdOrPassport = request.NationalIdOrPassport;

                // Update Categories (many-to-many)
                operatorEntity.Categories.Clear();
                operatorEntity.Categories = categories;

                await _context.SaveChangesAsync();
                // ✅ Send email after update
                var emailSubject = "Profile Updated Successfully";
                var emailBody = $@"
        Hello {operatorEntity.FirstName},<br/><br/>
        Your operator profile has been updated successfully.<br/>
        If you did not make this change, please contact support immediately.<br/><br/>
        Regards,<br/>
        NIB Team";

                await _emailService.SendEmailAsync(operatorEntity.User.Email, emailSubject, emailBody);
                await transaction.CommitAsync();

                return new OperatorResponse
                {
                    Id = operatorEntity.Id,
                    FirstName = operatorEntity.FirstName,
                    FatherName = operatorEntity.FatherName,
                    GrandFatherName = operatorEntity.GrandFatherName,
                    Email = operatorEntity.User.Email,
                    DateOfBirth = operatorEntity.DateOfBirth,
                    Gender = (Gender)operatorEntity.Gender,
                    PhoneNumber = operatorEntity.PhoneNumber,
                    Region = operatorEntity.Region,
                    City = operatorEntity.City,
                    SubCity = operatorEntity.SubCity,
                    NationalIdOrPassport = operatorEntity.NationalIdOrPassport,
                    CreatedAt = operatorEntity.CreatedAt,
                    CategoryNames = categories.Select(c => c.Name).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeleteOperatorAsync(Guid operatorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var operatorEntity = await _context.Operators
                    .Include(o => o.User)
                    .Include(o => o.Categories)
                    .FirstOrDefaultAsync(o => o.Id == operatorId);

                if (operatorEntity == null)
                    throw new Exception("Operator not found.");

                operatorEntity.Categories.Clear();

                _context.Users.Remove(operatorEntity.User);
                _context.Operators.Remove(operatorEntity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<OperatorResponse>> GetUnassignedOperatorsAsync()
        {
            var unassigned = await _context.Operators
                .Include(o => o.Categories)
                .Include(o => o.User)
                .Where(o => !o.Categories.Any())
                .ToListAsync();

            return unassigned.Select(op => new OperatorResponse
            {
                Id = op.Id,
                FirstName = op.FirstName,
                FatherName = op.FatherName,
                GrandFatherName = op.GrandFatherName,
                Email = op.User.Email,
                DateOfBirth = op.DateOfBirth,
                Gender = (Gender)op.Gender,
                PhoneNumber = op.PhoneNumber,
                Region = op.Region,
                City = op.City,
                SubCity = op.SubCity,
                NationalIdOrPassport = op.NationalIdOrPassport,
                CreatedAt = op.CreatedAt,
                CategoryNames = new List<string>()
            });
        }
        //Admin things end here




        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new Exception("New password is required.");

            if (request.NewPassword != request.ConfirmPassword)
                throw new Exception("Passwords do not match.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found.");

            var hasher = new PasswordHasher<User>();

            var verifyResult = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword
            );

            if (verifyResult == PasswordVerificationResult.Failed)
                throw new Exception("Current password is incorrect.");

            user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

            await _context.SaveChangesAsync();
        }
        public async Task<OperatorResponse?> GetMyProfileAsync(Guid userId)
        {
            var client = await _context.Operators
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null) return null;

            return new OperatorResponse
            {
                Id = client.Id,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                Gender = (Gender)client.Gender,
                NationalIdOrPassport = client.NationalIdOrPassport,
                LogoImageUrl = client.LogoImageUrl,
                //PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,
                Email = client.User?.Email ?? string.Empty,
                PhoneNumber = client.PhoneNumber,
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                CreatedAt = client.CreatedAt
            };
        }
        public async Task<IEnumerable<ClaimResponse>> GetAllClaimsWithClientInfoAsync()
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .ThenInclude(cl => cl.User) // Include user's email if needed
                .Include(c => c.MotorInsuranceApplication)
                .Include(c => c.LifeInsuranceApplication)
                .ToListAsync();

            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                ClientFirstName = c.Client.FirstName,
                ClientFatherName = c.Client.FatherName,
                ClientGrandFatherName = c.Client.GrandFatherName,
                ClientEmail = c.Client.User.Email,
                MotorInsuranceApplicationId = c.MotorInsuranceApplicationId != Guid.Empty ? c.MotorInsuranceApplicationId : null,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId != Guid.Empty ? c.LifeInsuranceApplicationId : null,
                IncidentDate = c.IncidentDate,
                IncidentTime = c.IncidentTime,
                Location = c.Location,
                IncidentType = c.IncidentType,
                Description = c.Description,
                EvidenceImageUrls = string.IsNullOrEmpty(c.EvidenceImagesJson)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(c.EvidenceImagesJson),
                TrafficPoliceReportPdfUrl = c.TrafficPoliceReportPdfUrl,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            });
        }
        public async Task<List<ClaimResponse>> GetAllLifeClaimsAsync()
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries)
                .Where(c => c.LifeInsuranceApplicationId != null && c.Status == ClaimStatus.Pending)
                .ToListAsync();

            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId.Value,
                IncidentDate = c.IncidentDate,
                ClaimReason = c.ClaimReason,
                DeathCertificatePdf = c.DeathCertificatePdf,
                MedicalReportPdf = c.MedicalReportPdf,
                HospitalDischargeSummaryPdf = c.HospitalDischargeSummaryPdf,
                HospitalName = c.HospitalName,
                Description = c.Description,
                Status = c.Status,
                CreatedAt = c.CreatedAt,

                // Client Info
                ClientEmail = c.ClientEmail,
                ClientFirstName = c.ClientFirstName,
                ClientFatherName = c.ClientFatherName,
                ClientGrandFatherName = c.ClientGrandFatherName,
                ApprovedAmount = c.ApprovedAmount,

                // Multiple beneficiaries returned as list
                Beneficiaries = c.LifeInsuranceApplication.Beneficiaries
                    .Select(b => new BeneficiaryResponse
                    {
                        Name = b.Name,
                        Email = b.Email,
                        Relation = b.Relation,
                        PhoneNumber = b.PhoneNumber,
                        NationalIdFilePath = b.NationalIdFilePath
                    }).ToList()

            }).ToList();
        }
        public async Task<ClaimResponse> GetClaimByIdAsync(Guid claimId)
        {
            var claim = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.MotorInsuranceApplication)
                .Include(c => c.LifeInsuranceApplication)
                .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
                throw new Exception("Claim not found.");

            // Map to ClaimResponse (same as in ReportMotorClaimAsync)
            var evidenceImages = string.IsNullOrEmpty(claim.EvidenceImagesJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(claim.EvidenceImagesJson);

            return new ClaimResponse
            {
                ClaimId = claim.Id,
                ClientId = claim.ClientId,
                MotorInsuranceApplicationId = claim.MotorInsuranceApplicationId,
                LifeInsuranceApplicationId = claim.LifeInsuranceApplicationId,
                IncidentDate = claim.IncidentDate,
                IncidentTime = claim.IncidentTime,
                Location = claim.Location,
                IncidentType = claim.IncidentType,
                Description = claim.Description,
                EvidenceImageUrls = evidenceImages,
                TrafficPoliceReportPdfUrl = claim.TrafficPoliceReportPdfUrl,
                Status = claim.Status,
                CreatedAt = claim.CreatedAt
            };
        }
        public async Task<ApproveClaimResponse> ReviewClaimAsync(Guid operatorUserId, Guid claimId, ReviewClaimRequest request)
        {
            // 1️⃣ Get operator
            var operatorEntity = await _context.Operators
                .FirstOrDefaultAsync(o => o.UserId == operatorUserId);

            if (operatorEntity == null)
                throw new Exception("Operator profile not found.");

            // 2️⃣ Get claim + relations
            var claim = await _context.Claims
                .Include(c => c.MotorInsuranceApplication)
                .Include(c => c.LifeInsuranceApplication)
                .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
                throw new Exception("Claim not found.");

            if (claim.Status != ClaimStatus.Pending)
                throw new Exception("Only pending claims can be reviewed.");

            decimal approvedAmount;

            // 3️⃣ Decide by claim type
            if (claim.MotorInsuranceApplicationId != null)
            {
                if (request == null || request.ApprovedAmount <= 0)
                    throw new Exception("Approved amount is required for motor claims.");

                approvedAmount = request.ApprovedAmount;
            }
            else if (claim.LifeInsuranceApplicationId != null)
            {
                approvedAmount = claim.LifeInsuranceApplication?.LifePrice
                    ?? throw new Exception("Life insurance price not found.");
            }
            else
            {
                throw new Exception("Unsupported claim type.");
            }

            // 4️⃣ Update claim
            claim.Status = ClaimStatus.ReviewedByOperator;
            claim.ApprovedAmount = approvedAmount;
            claim.OperatorId = operatorEntity.Id;
            claim.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApproveClaimResponse
            {
                ClaimId = claim.Id,
                Status = claim.Status,
                ApprovedAmount = approvedAmount,
                ApprovedAt = claim.ApprovedAt.Value
            };
        }
        public async Task<RejectClaimResponse> RejectClaimAsync(Guid operatorUserId, Guid claimId, RejectClaimRequest request)
        {
            // 1️⃣ Operator
            var operatorEntity = await _context.Operators
                .FirstOrDefaultAsync(o => o.UserId == operatorUserId);

            if (operatorEntity == null)
                throw new Exception("Operator profile not found.");

            // 2️⃣ Claim
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
                throw new Exception("Claim not found.");

            if (claim.Status != ClaimStatus.Pending)
                throw new Exception("Only pending claims can be rejected.");

            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                throw new Exception("Rejection reason is required.");

            // 3️⃣ Reject (insurance-type agnostic)
            claim.Status = ClaimStatus.Rejected;
            claim.OperatorId = operatorEntity.Id;
            claim.ApprovedAmount = null;
            claim.RejectionReason = request.RejectionReason;
            claim.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new RejectClaimResponse
            {
                ClaimId = claim.Id,
                RejectedByOperatorId = operatorEntity.Id,
                Status = claim.Status,
                RejectReason = claim.RejectionReason,
                RejectedAt = claim.ApprovedAt!.Value
            };
        }
        public async Task<List<ApproveClaimResponse>> GetReviewedClaimsByOperatorAsync(Guid operatorUserId)
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .Where(c => c.OperatorId == operatorUserId &&
                            c.Status == ClaimStatus.ReviewedByOperator)
                .OrderByDescending(c => c.ApprovedAt)
                .ToListAsync();

            return claims.Select(c => new ApproveClaimResponse
            {
                ClaimId = c.Id,
                Status = c.Status,
                ApprovedAmount = c.ApprovedAmount ?? 0,
                ApprovedAt = c.ApprovedAt ?? DateTime.MinValue
            }).ToList();
        }
        public async Task<List<ClaimResponse>> GetAllReviewedLifeClaimsAsync()
        {
            var claims = await _context.Claims
                .Where(c => c.LifeInsuranceApplicationId != null && c.Status == ClaimStatus.ReviewedByOperator)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.User)
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries)
                .ToListAsync();

            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId!.Value,
                IncidentDate = c.IncidentDate,
                ClaimReason = c.ClaimReason,
                ApprovedAmount = c.ApprovedAmount,
                Status = c.Status,
                CreatedAt = c.CreatedAt,

                // Client Info
                ClientFirstName = c.Client.FirstName,
                ClientFatherName = c.Client.FatherName,
                ClientGrandFatherName = c.Client.GrandFatherName,
                ClientEmail = c.Client.User.Email,

                // Beneficiaries
                Beneficiaries = c.LifeInsuranceApplication!.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()
            }).ToList();
        }
        public async Task<List<RejectClaimResponse>> GetAllRejectedClaimsAsync(Guid operatorUserId)
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.Operator)
                .Where(c => c.OperatorId == operatorUserId &&
                            c.Status == ClaimStatus.Rejected)
                .OrderByDescending(c => c.ApprovedAt)
                .ToListAsync();

            return claims.Select(c => new RejectClaimResponse
            {
                ClaimId = c.Id,
                RejectedByOperatorId = c.OperatorId,
                Status = c.Status,
                RejectReason = c.RejectReason,
                RejectedAt = c.ApprovedAt ?? DateTime.MinValue
            }).ToList();
        }
        public async Task<List<ClaimResponse>> GetAllRejectedLifeClaimsAsync()
        {
            var claims = await _context.Claims
                .Where(c => c.LifeInsuranceApplicationId != null && c.Status == ClaimStatus.Rejected)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.User)
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries)
                .ToListAsync();

            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId!.Value,
                IncidentDate = c.IncidentDate,
                ClaimReason = c.ClaimReason,
                RejectionReason = c.RejectionReason,
                Status = c.Status,
                CreatedAt = c.CreatedAt,

                // Client Info
                ClientFirstName = c.Client.FirstName,
                ClientFatherName = c.Client.FatherName,
                ClientGrandFatherName = c.Client.GrandFatherName,
                ClientEmail = c.Client.User.Email,

                // Beneficiaries
                Beneficiaries = c.LifeInsuranceApplication!.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()
            }).ToList();
        }
        public async Task<ApproveClaimResponse> GetReviewedClaimByIdAsync(Guid operatorUserId, Guid claimId)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.Id == claimId &&
                                          c.OperatorId == operatorUserId &&
                                          c.Status == ClaimStatus.ReviewedByOperator);

            if (claim == null)
                throw new Exception("Reviewed claim not found.");

            return new ApproveClaimResponse
            {
                ClaimId = claim.Id,
                Status = claim.Status,
                ApprovedAmount = claim.ApprovedAmount ?? 0,
                ApprovedAt = claim.ApprovedAt ?? DateTime.MinValue
            };
        }
        public async Task<OperatorDashboardResponse> GetOperatorDashboardAsync(Guid operatorUserId)
        {
            // 1️⃣ Resolve Operator
            var operatorEntity = await _context.Operators
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.UserId == operatorUserId);

            if (operatorEntity == null)
                throw new Exception("Operator profile not found.");

            var operatorId = operatorEntity.Id;

            // 2️⃣ Claims waiting for review (NO operator assigned yet)
            var pendingClaims = await _context.Claims
                .CountAsync(c => c.Status == ClaimStatus.Pending);

            // 3️⃣ Approved by this operator
            var approvedClaims = await _context.Claims
                .CountAsync(c => c.OperatorId == operatorId &&
                                 c.Status == ClaimStatus.ReviewedByOperator);

            // 4️⃣ Rejected by this operator
            var rejectedClaims = await _context.Claims
                .CountAsync(c => c.OperatorId == operatorId &&
                                 c.Status == ClaimStatus.Rejected);

            // 5️⃣ Total handled by this operator
            var totalHandled = approvedClaims + rejectedClaims;

            return new OperatorDashboardResponse
            {
                TotalClaims = totalHandled,
                ApprovedByOperator = approvedClaims,
                RejectedClaims = rejectedClaims,
                PendingClaims = pendingClaims
            };
        }



    }
}




