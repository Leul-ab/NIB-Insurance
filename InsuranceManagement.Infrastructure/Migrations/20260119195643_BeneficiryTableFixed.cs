using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BeneficiryTableFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add a temporary column to hold integer values
            migrationBuilder.AddColumn<int>(
                name: "RelationTemp",
                table: "Beneficiaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Step 2: Map old string values to integers using SQL CASE
            migrationBuilder.Sql(@"
        UPDATE ""Beneficiaries""
        SET ""RelationTemp"" = CASE ""Relation""
            WHEN 'Father' THEN 0
            WHEN 'Mother' THEN 1
            WHEN 'Spouse' THEN 2
            WHEN 'Child' THEN 3
            WHEN 'Sibling' THEN 4
            ELSE 0
        END;
    ");

            // Step 3: Drop old column
            migrationBuilder.DropColumn(name: "Relation", table: "Beneficiaries");

            // Step 4: Rename temp column to Relation
            migrationBuilder.RenameColumn("RelationTemp", "Beneficiaries", "Relation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse: create string column and map back integers to string
            migrationBuilder.AddColumn<string>(
                name: "RelationTemp",
                table: "Beneficiaries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
        UPDATE ""Beneficiaries""
        SET ""RelationTemp"" = CASE ""Relation""
            WHEN 0 THEN 'Father'
            WHEN 1 THEN 'Mother'
            WHEN 2 THEN 'Spouse'
            WHEN 3 THEN 'Child'
            WHEN 4 THEN 'Sibling'
            ELSE 'Father'
        END;
    ");

            migrationBuilder.DropColumn("Relation", "Beneficiaries");
            migrationBuilder.RenameColumn("RelationTemp", "Beneficiaries", "Relation");
        }

    }
}
