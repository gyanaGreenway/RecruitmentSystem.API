using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystem.API.Migrations
{
    public partial class AddOnboardingExtras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(name: "CreatedAt", table: "Onboardings", type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()");
            migrationBuilder.AddColumn<DateTime?>(name: "UpdatedAt", table: "Onboardings", type: "datetime2", nullable: true);

            migrationBuilder.AddColumn<int>(name: "Status", table: "OnboardingTasks", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<DateTime>(name: "CreatedAt", table: "OnboardingTasks", type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()");
            migrationBuilder.AddColumn<DateTime?>(name: "UpdatedAt", table: "OnboardingTasks", type: "datetime2", nullable: true);

            migrationBuilder.AddColumn<string>(name: "FileType", table: "OnboardingDocuments", type: "nvarchar(128)", nullable: true);
            migrationBuilder.AddColumn<long>(name: "FileSize", table: "OnboardingDocuments", type: "bigint", nullable: true);
            migrationBuilder.AddColumn<int>(name: "Status", table: "OnboardingDocuments", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<string>(name: "Comments", table: "OnboardingDocuments", type: "nvarchar(max)", nullable: false, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CreatedAt", table: "Onboardings");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "Onboardings");

            migrationBuilder.DropColumn(name: "Status", table: "OnboardingTasks");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "OnboardingTasks");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "OnboardingTasks");

            migrationBuilder.DropColumn(name: "FileType", table: "OnboardingDocuments");
            migrationBuilder.DropColumn(name: "FileSize", table: "OnboardingDocuments");
            migrationBuilder.DropColumn(name: "Status", table: "OnboardingDocuments");
            migrationBuilder.DropColumn(name: "Comments", table: "OnboardingDocuments");
        }
    }
}
