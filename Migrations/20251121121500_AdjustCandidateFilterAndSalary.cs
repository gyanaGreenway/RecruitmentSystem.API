using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystem.API.Migrations
{
    public partial class AdjustCandidateFilterAndSalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure Salary column precision
            migrationBuilder.AlterColumn<decimal>(
                name: "Salary",
                table: "Jobs",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op (precision stays)
        }
    }
}
