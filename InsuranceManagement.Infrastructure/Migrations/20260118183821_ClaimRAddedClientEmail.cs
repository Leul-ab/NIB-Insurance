using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClaimRAddedClientEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientEmail",
                table: "Claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientFatherName",
                table: "Claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientFirstName",
                table: "Claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientGrandFatherName",
                table: "Claims",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientEmail",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClientFatherName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClientFirstName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ClientGrandFatherName",
                table: "Claims");
        }
    }
}
