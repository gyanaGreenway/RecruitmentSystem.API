using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OnboardingTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OnboardingTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OnboardingTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "OnboardingDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "OnboardingDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "OnboardingDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OnboardingDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OnboardingTasks");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OnboardingDocuments");
        }
    }
}
