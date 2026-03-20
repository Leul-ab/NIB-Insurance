using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RejectReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Claims",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Claims");
        }
    }
}
