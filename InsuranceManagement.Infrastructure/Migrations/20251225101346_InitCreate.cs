using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovedByOperatorId",
                table: "Claims",
                newName: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_OperatorId",
                table: "Claims",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Operators_OperatorId",
                table: "Claims",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Operators_OperatorId",
                table: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_Claims_OperatorId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "OperatorId",
                table: "Claims",
                newName: "ApprovedByOperatorId");
        }
    }
}
