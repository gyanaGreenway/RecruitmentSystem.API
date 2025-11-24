using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystem.API.Migrations
{
    public partial class AddInterviewsManual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Interviews table if missing
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.Interviews','U') IS NULL BEGIN
CREATE TABLE dbo.Interviews(
  Id INT IDENTITY(1,1) PRIMARY KEY,
  PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
  CandidateId INT NOT NULL,
  JobId INT NOT NULL,
  Stage NVARCHAR(128) NOT NULL DEFAULT(''),
  StartUtc DATETIME2 NOT NULL,
  DurationMinutes INT NOT NULL,
  Timezone NVARCHAR(64) NOT NULL DEFAULT('UTC'),
  LocationType INT NOT NULL,
  MeetingProvider NVARCHAR(32) NOT NULL DEFAULT(''),
  MeetingLink NVARCHAR(512) NULL,
  LocationDetail NVARCHAR(512) NULL,
  Notes NVARCHAR(MAX) NOT NULL DEFAULT(''),
  Reminders NVARCHAR(512) NOT NULL DEFAULT(''),
  Flags NVARCHAR(512) NOT NULL DEFAULT(''),
  InterviewerIds NVARCHAR(512) NOT NULL DEFAULT(''),
  InterviewerNames NVARCHAR(1024) NOT NULL DEFAULT(''),
  ExternalEventId NVARCHAR(256) NOT NULL DEFAULT(''),
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  UpdatedAt DATETIME2 NULL
);
CREATE UNIQUE INDEX IX_Interviews_PublicId ON dbo.Interviews(PublicId);
CREATE INDEX IX_Interviews_CandidateId_JobId_Stage ON dbo.Interviews(CandidateId, JobId, Stage);
END");

            // Create InterviewFeedbacks table if missing
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.InterviewFeedbacks','U') IS NULL BEGIN
CREATE TABLE dbo.InterviewFeedbacks(
  Id INT IDENTITY(1,1) PRIMARY KEY,
  PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
  InterviewId INT NOT NULL,
  CandidateId INT NOT NULL,
  CandidateName NVARCHAR(256) NOT NULL DEFAULT(''),
  JobId INT NOT NULL,
  JobTitle NVARCHAR(256) NOT NULL DEFAULT(''),
  Stage NVARCHAR(128) NOT NULL DEFAULT(''),
  InterviewerId INT NOT NULL,
  InterviewerName NVARCHAR(256) NOT NULL DEFAULT(''),
  TimestampUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  Score INT NOT NULL DEFAULT(0),
  Verdict NVARCHAR(32) NOT NULL DEFAULT(''),
  Strengths NVARCHAR(1024) NOT NULL DEFAULT(''),
  Reservations NVARCHAR(1024) NOT NULL DEFAULT(''),
  Notes NVARCHAR(MAX) NOT NULL DEFAULT(''),
  Status NVARCHAR(64) NOT NULL DEFAULT(''),
  NextActions NVARCHAR(512) NOT NULL DEFAULT(''),
  CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE UNIQUE INDEX IX_InterviewFeedbacks_PublicId ON dbo.InterviewFeedbacks(PublicId);
CREATE INDEX IX_InterviewFeedbacks_InterviewId_InterviewerId ON dbo.InterviewFeedbacks(InterviewId, InterviewerId);
END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.InterviewFeedbacks','U') IS NOT NULL DROP TABLE dbo.InterviewFeedbacks;");
            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.Interviews','U') IS NOT NULL DROP TABLE dbo.Interviews;");
        }
    }
}
