using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BeneificaryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryName",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryNationalIdPath",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryPhoneNumber",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BeneficiaryRelation",
                table: "LifeInsuranceApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficiaryName",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryNationalIdPath",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryPhoneNumber",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryRelation",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "LifeInsuranceApplications");
        }
    }
}
