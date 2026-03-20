using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectFieldsToClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "Claims",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "Claims");
        }
    }
}
