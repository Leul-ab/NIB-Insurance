using InsuranceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public static class EmailTemplateService
    {
        public static string GenerateLifeSubmittedEmail(LifeInsuranceApplication app, string secretKey)
        {
            return $@"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color:#2E86C1;'>Life Insurance Application Submitted</h2>
    <p>Dear <strong>{app.Client.FirstName}</strong>,</p>
    <p>Your life insurance application has been successfully submitted for review.</p>
    <p><strong>Secret Key:</strong> {secretKey}</p>
    <p>You will be notified once the review is complete.</p>
    <p style='color:#666;'>- NIB Insurance</p>
</body>
</html>";
        }

        public static string GenerateBeneficiaryAddedEmail(LifeInsuranceBeneficiary b, string secretKey)
        {
            return $@"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color:#2E86C1;'>You Have Been Added as a Beneficiary</h2>
    <p>Hello <strong>{b.Name}</strong>,</p>
    <p>You have been listed as a beneficiary on a life insurance policy.</p>
    <p><strong>Your secret verification key:</strong></p>
    <h3 style='color:#D35400;'>{secretKey}</h3>
    <p>This key is required to verify or claim benefits.</p>
    <p style='color:#666;'>- NIB Insurance</p>
</body>
</html>";
        }
    }

}
