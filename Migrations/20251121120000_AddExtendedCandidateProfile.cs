using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystem.API.Migrations
{
    public partial class AddExtendedCandidateProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add extended Candidate columns
            migrationBuilder.AddColumn<string>(name: "ResumeHeadline", table: "Candidates", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "ProfileSummary", table: "Candidates", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "Accomplishments", table: "Candidates", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "CareerProfile", table: "Candidates", type: "nvarchar(max)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<DateTime>(name: "DateOfBirth", table: "Candidates", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Gender", table: "Candidates", type: "nvarchar(32)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "Nationality", table: "Candidates", type: "nvarchar(128)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "MaritalStatus", table: "Candidates", type: "nvarchar(32)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "Address", table: "Candidates", type: "nvarchar(512)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "City", table: "Candidates", type: "nvarchar(128)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "State", table: "Candidates", type: "nvarchar(128)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "ZipCode", table: "Candidates", type: "nvarchar(32)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "Country", table: "Candidates", type: "nvarchar(128)", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<bool>(name: "IsDeleted", table: "Candidates", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<byte[]>(name: "RowVersion", table: "Candidates", type: "rowversion", rowVersion: true, nullable: true);

            // Create employment table
            migrationBuilder.CreateTable(
                name: "CandidateEmployment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    Company = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    WorkArea = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentlyWorking = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateEmployment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEmployment_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(name: "IX_CandidateEmployment_CandidateId", table: "CandidateEmployment", column: "CandidateId");

            // Create education table
            migrationBuilder.CreateTable(
                name: "CandidateEducation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    Field = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    Institution = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(128)", nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateEducation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEducation_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(name: "IX_CandidateEducation_CandidateId", table: "CandidateEducation", column: "CandidateId");

            // Create skills table
            migrationBuilder.CreateTable(
                name: "CandidateSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    Skill = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    Proficiency = table.Column<string>(type: "nvarchar(64)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateSkills_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(name: "IX_CandidateSkills_CandidateId", table: "CandidateSkills", column: "CandidateId");

            // Create projects table
            migrationBuilder.CreateTable(
                name: "CandidateProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", nullable: false, defaultValue: ""),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Link = table.Column<string>(type: "nvarchar(512)", nullable: false, defaultValue: ""),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateProjects_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(name: "IX_CandidateProjects_CandidateId", table: "CandidateProjects", column: "CandidateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CandidateEmployment");
            migrationBuilder.DropTable(name: "CandidateEducation");
            migrationBuilder.DropTable(name: "CandidateSkills");
            migrationBuilder.DropTable(name: "CandidateProjects");

            migrationBuilder.DropColumn(name: "ResumeHeadline", table: "Candidates");
            migrationBuilder.DropColumn(name: "ProfileSummary", table: "Candidates");
            migrationBuilder.DropColumn(name: "Accomplishments", table: "Candidates");
            migrationBuilder.DropColumn(name: "CareerProfile", table: "Candidates");
            migrationBuilder.DropColumn(name: "DateOfBirth", table: "Candidates");
            migrationBuilder.DropColumn(name: "Gender", table: "Candidates");
            migrationBuilder.DropColumn(name: "Nationality", table: "Candidates");
            migrationBuilder.DropColumn(name: "MaritalStatus", table: "Candidates");
            migrationBuilder.DropColumn(name: "Address", table: "Candidates");
            migrationBuilder.DropColumn(name: "City", table: "Candidates");
            migrationBuilder.DropColumn(name: "State", table: "Candidates");
            migrationBuilder.DropColumn(name: "ZipCode", table: "Candidates");
            migrationBuilder.DropColumn(name: "Country", table: "Candidates");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Candidates");
            migrationBuilder.DropColumn(name: "RowVersion", table: "Candidates");
        }
    }
}
