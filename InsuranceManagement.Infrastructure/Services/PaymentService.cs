using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace InsuranceManagement.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly InsuranceDbContext _context;
        private readonly string _chapaSecretKey;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public PaymentService(InsuranceDbContext context, IConfiguration config, IPasswordHasher<User> passwordHasher, IEmailService emailService)
        {
            _context = context;
            _chapaSecretKey = config["Chapa:SecretKey"];
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }


        public async Task<PaymentInitializeResponse> InitializePaymentAsync(Guid applicationId)
        {
            decimal amount = 0;
            string clientEmail = "";
            string clientFirstName = "";
            string clientLastName = "";
            string appType = ""; // "motor" or "life"

            // --- Try find Motor application ---
            var motorApp = await _context.MotorInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (motorApp != null)
            {
                if (motorApp.Status?.ToLower() != "approved")
                    throw new Exception("Motor insurance application not approved");

                amount = motorApp.CalculatedPremium;
                clientEmail = motorApp.Client.User.Email;
                clientFirstName = motorApp.Client.FirstName;
                clientLastName = motorApp.Client.FatherName;
                appType = "motor";

                motorApp.PaymentReference = Guid.NewGuid().ToString();
                await _context.SaveChangesAsync();
            }
            else
            {
                // --- Try find Life application ---
                var lifeApp = await _context.LifeInsuranceApplications
                    .Include(a => a.Client).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (lifeApp == null)
                    throw new Exception("Application not found");

                if (lifeApp.Status?.ToLower() != "approved")
                    throw new Exception("Life insurance application not approved");

                amount = lifeApp.LifePrice;
                clientEmail = lifeApp.Client.User.Email;
                clientFirstName = lifeApp.Client.FirstName;
                clientLastName = lifeApp.Client.FatherName;
                appType = "life";

                lifeApp.PaymentReference = Guid.NewGuid().ToString();
                await _context.SaveChangesAsync();
            }

            var reference = Guid.NewGuid().ToString();

            var payload = new
            {
                amount = amount.ToString("F2", CultureInfo.InvariantCulture),
                currency = "ETB",
                email = clientEmail,
                first_name = clientFirstName,
                last_name = clientLastName,
                tx_ref = reference,
                callback_url = "https://your-backend.com/api/payments/chapa/callback",
                return_url = "https://your-frontend.com/payment-success",

                customization = new
                {
                    title = "InsurancePay",
                    description = appType == "motor"
                        ? "Premium payment for motor insurance"
                        : "Premium payment for life insurance"
                }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_chapaSecretKey}");

            var response = await client.PostAsJsonAsync("https://api.chapa.co/v1/transaction/initialize", payload);
            var rawJson = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Chapa Response JSON: " + rawJson);

            var chapaResponse = JsonSerializer.Deserialize<ChapaInitResponse>(
                rawJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (chapaResponse == null || chapaResponse.Status != "success")
                throw new Exception($"Chapa initialization failed. Raw response: {rawJson}");

            // Save reference for the correct application type
            if (motorApp != null)
                motorApp.PaymentReference = reference;
            else
                _context.LifeInsuranceApplications
                    .First(a => a.Id == applicationId).PaymentReference = reference;

            await _context.SaveChangesAsync();

            return new PaymentInitializeResponse
            {
                CheckoutUrl = chapaResponse.Data.Checkout_Url,
                Reference = reference
            };
        }

        public async Task<string> VerifyPaymentAsync(string txRef)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_chapaSecretKey}");

            var response = await client.GetAsync($"https://api.chapa.co/v1/transaction/verify/{txRef}");
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("VERIFY RAW JSON: " + json);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Chapa verification failed: {json}");

            var verify = JsonSerializer.Deserialize<ChapaVerifyResponse>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var paymentStatus = verify?.Data?.Status?.ToLower();

            if (paymentStatus == "success")
            {
                string emailTo = null;
                string clientName = null;
                string policyType = null;
                string policyId = null;

                // 1️⃣ Try to find motor insurance application
                var motorApp = await _context.MotorInsuranceApplications
                    .Include(a => a.Client).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.PaymentReference == txRef);

                if (motorApp != null)
                {
                    motorApp.IsPaid = true;
                    motorApp.PaymentStatus = "Paid";
                    await _context.SaveChangesAsync();

                    emailTo = motorApp.Client.User.Email;
                    clientName = motorApp.Client.FirstName;
                    policyType = "Motor Insurance";
                    policyId = motorApp.Id.ToString();

                    Console.WriteLine("✔ Motor insurance payment verified");
                }

                // 2️⃣ Try to find life insurance application
                var lifeApp = await _context.LifeInsuranceApplications
                    .Include(a => a.Client).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(a => a.PaymentReference == txRef);

                if (lifeApp != null)
                {
                    lifeApp.IsPaid = true;
                    lifeApp.PaymentStatus = "Paid";
                    await _context.SaveChangesAsync();

                    emailTo = lifeApp.Client.User.Email;
                    clientName = lifeApp.Client.FirstName;
                    policyType = "Life Insurance";
                    policyId = lifeApp.Id.ToString();

                    Console.WriteLine("✔ Life insurance payment verified");
                }

                // 3️⃣ Send payment confirmation email if a client was found
                if (!string.IsNullOrEmpty(emailTo))
                {
                    var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Payment Successful</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>{policyType} Payment Successful</h2>
        <p>Dear <strong>{clientName}</strong>,</p>
        <p>We are pleased to inform you that your {policyType.ToLower()} policy (ID: <strong>{policyId}</strong>) has been successfully paid.</p>

        <h3 style='color: #2E86C1;'>Payment Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Policy Type</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{policyType}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Policy ID</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{policyId}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Transaction Reference</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{txRef}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Payment Status</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>Paid</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>Thank you for your payment. Your policy is now active.</p>
        <p style='margin-top: 30px;'>Best regards,<br /><strong>Your Insurance Company Name</strong></p>
    </div>
</body>
</html>
";

                    await _emailService.SendEmailAsync(emailTo, $"{policyType} Payment Successful", emailBody);
                }

                return json;
            }
            else
            {
                Console.WriteLine($"🚫 Payment NOT completed. Status: {paymentStatus}");
            }

            return json;
        }

        public async Task<List<PaymentTransactionResponse>> GetRecentTransactionsAsync()
        {
            // 1️⃣ Get Motor transactions
            var motorTx = await _context.MotorInsuranceApplications
                .Include(a => a.Client)
                .Select(a => new PaymentTransactionResponse
                {
                    ApplicationId = a.Id,
                    Reference = a.PaymentReference,
                    ClientId = a.Client.Id,
                    ClientFullName = $"{a.Client.FirstName} {a.Client.FatherName}",
                    PremiumAmount = a.CalculatedPremium,
                    PaymentStatus = a.PaymentStatus,
                    IsPaid = a.IsPaid,
                    CreatedAt = a.CreatedAt,
                    InsuranceType = "Motor"
                })
                .ToListAsync();

            // 2️⃣ Get Life transactions
            var lifeTx = await _context.LifeInsuranceApplications
                .Include(a => a.Client)
                .Select(a => new PaymentTransactionResponse
                {
                    ApplicationId = a.Id,
                    Reference = a.PaymentReference,
                    ClientId = a.Client.Id,
                    ClientFullName = $"{a.Client.FirstName} {a.Client.FatherName}",
                    PremiumAmount = a.LifePrice,
                    PaymentStatus = a.PaymentStatus,
                    IsPaid = a.IsPaid,
                    CreatedAt = a.CreatedAt,
                    InsuranceType = "Life"
                })
                .ToListAsync();

            // 3️⃣ Combine both lists, order by date, return last 20
            return motorTx
                .Concat(lifeTx)
                .OrderByDescending(t => t.CreatedAt)
                .Take(20)
                .ToList();
        } // for finance team dashboard

        public async Task<List<ClaimPayoutResponse>> PayoutApprovedClaimAsync(Guid claimId)
        {
            var claim = await _context.Claims
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.User)
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries)
                .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
                throw new Exception("Claim not found");

            if (claim.Status != ClaimStatus.ApprovedByManager)
                throw new Exception("Claim is not approved by manager.");

            if (claim.ApprovedAmount == null || claim.ApprovedAmount <= 0)
                throw new Exception("Invalid approved amount.");

            if (claim.PayoutStatus == ClaimPayoutStatus.Paid)
                throw new Exception("Claim already paid.");

            // Mark as processing
            claim.PayoutStatus = ClaimPayoutStatus.Processing;
            await _context.SaveChangesAsync();

            var payoutReference = $"CLAIM-PAYOUT-{Guid.NewGuid()}";
            var payoutResponses = new List<ClaimPayoutResponse>();

            try
            {
                bool isLifeClaim = claim.LifeInsuranceApplicationId != null;

                if (isLifeClaim)
                {
                    var lifeApp = claim.LifeInsuranceApplication!;
                    if (lifeApp.Beneficiaries == null || !lifeApp.Beneficiaries.Any())
                        throw new Exception("No beneficiaries found for this life insurance claim.");

                    foreach (var b in lifeApp.Beneficiaries)
                    {
                        string payoutName = b.Name;
                        string payoutPhone = b.PhoneNumber;
                        string emailTo = b.Email;

                        // Simulated payout
                        await Task.Delay(500);

                        // Send email to each beneficiary
                        var beneficiaryEmailBody = $@"
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>Payout Confirmation</h2>
        <p>Dear <strong>{payoutName}</strong>,</p>
        <p>We would like to inform you that the approved insurance claim payout associated with:</p>
        <ul>
            <li><strong>Claim ID:</strong> {claim.Id}</li>
            <li><strong>Payout Reference:</strong> {payoutReference}</li>
            <li><strong>Amount:</strong> {claim.ApprovedAmount:C}</li>
        </ul>
        <p>has been successfully received on your behalf.</p>
        <p style='margin-top: 20px;'>If this information is incorrect, please contact support immediately.</p>
        <p style='margin-top: 30px;'>Best Regards,<br><strong>Your Insurance Company</strong></p>
    </div>
</body>
</html>";

                        if (!string.IsNullOrWhiteSpace(emailTo))
                            await _emailService.SendEmailAsync(emailTo, "Insurance Payout Confirmation", beneficiaryEmailBody);

                        payoutResponses.Add(new ClaimPayoutResponse
                        {
                            ClaimId = claim.Id,
                            ClientFullName = payoutName,
                            ClientPhoneNumber = payoutPhone,
                            ReceiverName = payoutName,
                            ReceiverPhoneNumber = payoutPhone,
                            ApprovedAmount = claim.ApprovedAmount.Value,
                            PaidAt = DateTime.UtcNow,
                            PayoutReference = payoutReference,
                            ClaimStatus = claim.Status.ToString(),
                            PayoutStatus = ClaimPayoutStatus.Paid.ToString()
                        });
                    }
                }
                else
                {
                    // Motor claim or single client payout
                    string payoutName = $"{claim.Client.FirstName} {claim.Client.FatherName} {claim.Client.GrandFatherName}";
                    string payoutPhone = claim.Client.PhoneNumber;
                    string emailTo = claim.Client.User.Email;

                    await Task.Delay(500);

                    var emailBody = $@"
<html>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>Claim Payout Received</h2>
        <p>Dear <strong>{payoutName}</strong>,</p>
        <p>Your approved claim (ID: <strong>{claim.Id}</strong>) payout has been successfully processed.</p>
        <ul>
            <li><strong>Payout Reference:</strong> {payoutReference}</li>
            <li><strong>Amount:</strong> {claim.ApprovedAmount:C}</li>
            <li><strong>Paid At:</strong> {DateTime.UtcNow:f}</li>
            <li><strong>Recipient Phone:</strong> {payoutPhone}</li>
        </ul>
        <p>If you have any questions, please contact support.</p>
        <p>Best Regards,<br/><strong>Your Insurance Company</strong></p>
    </div>
</body>
</html>";

                    if (!string.IsNullOrWhiteSpace(emailTo))
                        await _emailService.SendEmailAsync(emailTo, "Claim Payout Received", emailBody);

                    payoutResponses.Add(new ClaimPayoutResponse
                    {
                        ClaimId = claim.Id,
                        ClientFullName = payoutName,
                        ClientPhoneNumber = payoutPhone,
                        ReceiverName = payoutName,
                        ReceiverPhoneNumber = payoutPhone,
                        ApprovedAmount = claim.ApprovedAmount.Value,
                        PaidAt = DateTime.UtcNow,
                        PayoutReference = payoutReference,
                        ClaimStatus = claim.Status.ToString(),
                        PayoutStatus = ClaimPayoutStatus.Paid.ToString()
                    });
                }

                // Update claim payout status once all emails are sent
                claim.PayoutStatus = ClaimPayoutStatus.Paid;
                claim.PaidAt = DateTime.UtcNow;
                claim.PayoutReference = payoutReference;
                claim.PayoutFailureReason = null;

                claim.Status = ClaimStatus.Paid;
                await _context.SaveChangesAsync();

                return payoutResponses;
            }
            catch (Exception ex)
            {
                claim.PayoutStatus = ClaimPayoutStatus.Failed;
                claim.PayoutFailureReason = ex.Message;
                claim.Status = ClaimStatus.PayoutFailed;
                await _context.SaveChangesAsync();
                throw;
            }
        }



        //for client 
        public async Task<List<PaymentTransactionResponse>> RecentTransactionsByUserAsync(Guid clientId)
        {
            // 1️⃣ Motor Transactions for this user
            var motorTx = await _context.MotorInsuranceApplications
                .Include(a => a.Client)
                .Where(a => a.ClientId == clientId && a.PaymentReference != null)
                .Select(a => new PaymentTransactionResponse
                {
                    ApplicationId = a.Id,
                    ClientId = a.Client.Id,
                    Reference = a.PaymentReference,
                    PaymentStatus = a.PaymentStatus,
                    PremiumAmount = a.CalculatedPremium,
                    IsPaid = a.IsPaid,
                    ClientFullName = $"{a.Client.FirstName} {a.Client.FatherName}",
                    CreatedAt = a.CreatedAt,
                    InsuranceType = "Motor"
                })
                .ToListAsync();

            // 2️⃣ Life Transactions for this user
            var lifeTx = await _context.LifeInsuranceApplications
                .Include(a => a.Client)
                .Where(a => a.ClientId == clientId && a.PaymentReference != null)
                .Select(a => new PaymentTransactionResponse
                {
                    ApplicationId = a.Id,
                    ClientId = a.Client.Id,
                    Reference = a.PaymentReference,
                    PaymentStatus = a.PaymentStatus,
                    PremiumAmount = a.LifePrice,
                    IsPaid = a.IsPaid,
                    ClientFullName = $"{a.Client.FirstName} {a.Client.FatherName}",
                    CreatedAt = a.CreatedAt,
                    InsuranceType = "Life"
                })
                .ToListAsync();

            // 3️⃣ Combine → Sort → Take 10
            return motorTx
                .Concat(lifeTx)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToList();
        }

        public async Task<List<MotorInsuranceApplicationResponse>> PaidApplicationsByUserAsync(Guid userId)
        {
            // Get the client's ID linked to this user
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null) return new List<MotorInsuranceApplicationResponse>();

            var paidApps = await _context.MotorInsuranceApplications
                .Where(a => a.ClientId == client.Id && a.IsPaid == true)
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return paidApps.Select(a => new MotorInsuranceApplicationResponse
            {
                ApplicationId = a.Id,
                ClientId = a.ClientId,
                CategoryName = a.Category?.Name ?? "",
                SubCategoryName = a.SubCategory?.Name ?? "",
                Model = a.Model,
                PlateNumber = a.PlateNumber,
                YearOfManufacture = a.YearOfManufacture,
                EngineNumber = a.EngineNumber,
                ChassisNumber = a.ChassisNumber,
                MarketPrice = a.MarketPrice,
                CalculatedPremium = a.CalculatedPremium,
                InsuranceType = a.InsuranceType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Message = a.Message ?? "",
                ClientFullName = $"{a.Client.FirstName} {a.Client.FatherName} {a.Client.GrandFatherName}",
                ClientEmail = a.Client.User.Email,
                ClientPhoneNumber = a.Client.PhoneNumber,
                ClientGender = (Gender)a.Client.Gender,
                ClientDateOfBirth = a.Client.DateOfBirth,
                ClientNationalIdOrPassport = a.Client.NationalIdOrPassport,
                ClientPassportOrNationalIdImageUrl = a.Client.PassportOrNationalIdImageUrl ?? "",
                CarImageUrl = a.CarImagePath,
                CarLibreImageUrl = a.CarLibreImagePath
            }).ToList();
        }

        public async Task<List<LifeInsuranceApplicationResponse>> PaidLifeApplicationsByUserAsync(Guid userId)
        {
            // Get the client's ID linked to this user
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null) return new List<LifeInsuranceApplicationResponse>();

            var paidApps = await _context.LifeInsuranceApplications
                .Where(a => a.ClientId == client.Id && a.IsPaid == true)
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Beneficiaries)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return paidApps.Select(a => new LifeInsuranceApplicationResponse
            {
                ApplicationId = a.Id,
                ClientId = a.ClientId,
                CategoryName = a.Category?.Name ?? "",
                SubCategoryName = a.SubCategory?.Name ?? "",
                Age = a.Age,
                Weight = a.Weight,
                Height = a.Height,
                LifePrice = a.LifePrice,
                LifeInsuranceType = a.LifeInsuranceType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Message = a.Message ?? "",
                ClientFullName = $"{a.Client.FirstName} {a.Client.FatherName} {a.Client.GrandFatherName}",
                ClientEmail = a.Client.User.Email,
                ClientPhoneNumber = a.Client.PhoneNumber,
                ClientGender = (Domain.Enums.Gender)a.Client.Gender,
                ClientDateOfBirth = a.Client.DateOfBirth,
                ClientNationalIdOrPassport = a.Client.NationalIdOrPassport,
                ClientPassportOrNationalIdImageUrl = a.Client.PassportOrNationalIdImageUrl ?? "",

                Beneficiaries = a.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()
            }).ToList();
        }

        public async Task<RevealSecretKeyResponse> RevealSecretKeyAsync(Guid userId, RevealSecretKeyRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found");

            // Verify password (assuming hashed)
            bool isValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Success;
            if (!isValid) throw new Exception("Invalid password");

            var app = await _context.LifeInsuranceApplications
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId && a.Client.UserId == userId);

            if (app == null) throw new Exception("Application not found");

            return new RevealSecretKeyResponse
            {
                ApplicationId = app.Id,
                SecretKey = app.SecretKey // 🔥 Only revealed here
            };
        }

        public async Task<List<ClaimPayoutHistoryResponse>> GetPayoutHistoryAsync()
        {
            var payouts = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries) // include multiple beneficiaries
                .Where(c => c.PayoutStatus == ClaimPayoutStatus.Paid)
                .OrderByDescending(c => c.PaidAt)
                .ToListAsync();

            var history = new List<ClaimPayoutHistoryResponse>();

            foreach (var c in payouts)
            {
                bool isLifeClaim = c.LifeInsuranceApplicationId != null;

                if (isLifeClaim)
                {
                    // For life claims, create a history entry per beneficiary
                    foreach (var b in c.LifeInsuranceApplication!.Beneficiaries)
                    {
                        history.Add(new ClaimPayoutHistoryResponse
                        {
                            ClaimId = c.Id,
                            ReceiverName = b.Name,
                            ReceiverPhoneNumber = b.PhoneNumber,
                            ApprovedAmount = c.ApprovedAmount ?? 0,
                            PaidAt = c.PaidAt!.Value,
                            PayoutReference = c.PayoutReference ?? "",
                            ClaimStatus = c.Status.ToString(),
                            PayoutStatus = c.PayoutStatus.ToString(),
                            ClaimDescription = c.Description,
                            IncidentDate = c.IncidentDate,
                            ClaimType = "Life"
                        });
                    }
                }
                else
                {
                    // Motor or other claim types
                    history.Add(new ClaimPayoutHistoryResponse
                    {
                        ClaimId = c.Id,
                        ClientFullName = $"{c.Client.FirstName} {c.Client.FatherName} {c.Client.GrandFatherName}",
                        ReceiverName = $"{c.Client.FirstName} {c.Client.FatherName} {c.Client.GrandFatherName}",
                        ReceiverPhoneNumber = c.Client.PhoneNumber,
                        ApprovedAmount = c.ApprovedAmount ?? 0,
                        PaidAt = c.PaidAt!.Value,
                        PayoutReference = c.PayoutReference ?? "",
                        ClaimStatus = c.Status.ToString(),
                        PayoutStatus = c.PayoutStatus.ToString(),
                        ClaimDescription = c.Description,
                        IncidentDate = c.IncidentDate,
                        ClaimType = "Motor"
                    });
                }
            }

            return history;
        }

        public async Task<List<ClaimPayoutHistoryResponse>> GetLifeClaimPayoutHistoryAsync(string? secretKey = null, string? beneficiaryPhone = null)
        {
            // Start query
            var query = _context.Claims
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries)
                .Where(c => c.PayoutStatus == ClaimPayoutStatus.Paid && c.LifeInsuranceApplicationId != null);

            // Filter by secret key if provided (for guest)
            if (!string.IsNullOrWhiteSpace(secretKey))
            {
                query = query.Where(c => c.LifeInsuranceApplication!.SecretKey == secretKey);
            }

            // Filter by beneficiary phone if provided
            if (!string.IsNullOrWhiteSpace(beneficiaryPhone))
            {
                query = query.Where(c => c.LifeInsuranceApplication!.Beneficiaries
                    .Any(b => b.PhoneNumber == beneficiaryPhone));
            }

            // Fetch from DB
            var claims = await query
                .OrderByDescending(c => c.PaidAt)
                .ToListAsync();

            // Flatten claims to beneficiary responses
            var result = claims
                .SelectMany(c => c.LifeInsuranceApplication!.Beneficiaries
                    .Where(b => string.IsNullOrWhiteSpace(beneficiaryPhone) || b.PhoneNumber == beneficiaryPhone)
                    .Select(b => new ClaimPayoutHistoryResponse
                    {
                        ClaimId = c.Id,
                        ClientFullName = $"{c.Client.FirstName} {c.Client.FatherName} {c.Client.GrandFatherName}",
                        //ClientFirstName = c.ClientFirstName,
                        //ClientFatherName = c.ClientFatherName,
                        //ClientGrandFatherName = c.ClientGrandFatherName,
                        ReceiverName = b.Name,
                        ReceiverPhoneNumber = b.PhoneNumber,
                        ApprovedAmount = c.ApprovedAmount ?? 0,
                        PaidAt = c.PaidAt!.Value,
                        PayoutReference = c.PayoutReference ?? "",
                        ClaimStatus = c.Status.ToString(),
                        PayoutStatus = c.PayoutStatus.ToString(),
                        ClaimDescription = c.Description,
                        IncidentDate = c.IncidentDate,
                        ClaimType = "Life"
                    }))
                .ToList();

            return result;
        }



        public class ChapaInitResponse
        {
            public string Status { get; set; }
            public ChapaInitData Data { get; set; }
        }

        public class ChapaInitData
        {
            public string Checkout_Url { get; set; }
            public string Reference { get; set; }
        }

        public class ChapaVerifyResponse
        {
            public string Status { get; set; }
            public ChapaVerifyData Data { get; set; }
        }

        public class ChapaVerifyData
        {
            public string Status { get; set; }
            public string Tx_Ref { get; set; }
        }
    }
}
