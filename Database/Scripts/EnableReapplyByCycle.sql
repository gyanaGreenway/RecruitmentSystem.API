-- Recruitment System DB Script: Enable Re-apply by Hiring Cycle and Utilities
-- Safe to run multiple times (idempotent) on SQL Server

SET NOCOUNT ON;
GO

--1) Ensure Jobs.Cycle column exists (default =1)
IF COL_LENGTH('dbo.Jobs','Cycle') IS NULL
BEGIN
 ALTER TABLE dbo.Jobs ADD Cycle int NULL;
 UPDATE dbo.Jobs SET Cycle =1 WHERE Cycle IS NULL;
 ALTER TABLE dbo.Jobs ADD CONSTRAINT DF_Jobs_Cycle DEFAULT(1) FOR Cycle;
 ALTER TABLE dbo.Jobs ALTER COLUMN Cycle int NOT NULL;
END
GO

--2) Ensure Applications.Cycle column exists (default =1)
IF COL_LENGTH('dbo.Applications','Cycle') IS NULL
BEGIN
 ALTER TABLE dbo.Applications ADD Cycle int NULL;
 UPDATE dbo.Applications SET Cycle =1 WHERE Cycle IS NULL;
 ALTER TABLE dbo.Applications ADD CONSTRAINT DF_Applications_Cycle DEFAULT(1) FOR Cycle;
 ALTER TABLE dbo.Applications ALTER COLUMN Cycle int NOT NULL;
END
GO

--3) Replace unique index on (JobId, CandidateId) with (JobId, CandidateId, Cycle)
-- Drop common index name if it exists
IF EXISTS (
 SELECT1
 FROM sys.indexes i
 WHERE i.object_id = OBJECT_ID('dbo.Applications')
 AND i.name = 'IX_Applications_JobId_CandidateId'
)
BEGIN
 DROP INDEX IX_Applications_JobId_CandidateId ON dbo.Applications;
END
GO
-- Create new unique index if not present
IF NOT EXISTS (
 SELECT1
 FROM sys.indexes i
 WHERE i.object_id = OBJECT_ID('dbo.Applications')
 AND i.name = 'IX_Applications_Job_Candidate_Cycle'
)
BEGIN
 CREATE UNIQUE INDEX IX_Applications_Job_Candidate_Cycle
 ON dbo.Applications(JobId, CandidateId, Cycle);
END
GO

--4) Stored procedure to reopen a job for a new hiring cycle
CREATE OR ALTER PROCEDURE dbo.IncrementJobCycle
 @JobId int
AS
BEGIN
 SET NOCOUNT ON;
 UPDATE dbo.Jobs
 SET Cycle = Cycle +1,
 IsActive =1,
 PostedDate = SYSUTCDATETIME()
 WHERE Id = @JobId;
END
GO

--5) Scalar function to decide if a candidate can re-apply in the current cycle
CREATE OR ALTER FUNCTION dbo.CanReapply(@JobId int, @CandidateId int)
RETURNS bit
AS
BEGIN
 DECLARE @can bit =1;
 -- block if an active application exists in the current cycle
 IF EXISTS (
 SELECT1
 FROM dbo.Applications a
 JOIN dbo.Jobs j ON j.Id = a.JobId
 WHERE a.JobId = @JobId
 AND a.CandidateId = @CandidateId
 AND a.Cycle = j.Cycle
 AND a.Status IN (1,2,4) -- New, Shortlisted, Hired
 )
 SET @can =0;
 RETURN @can;
END
GO

--6) Optional: Ensure Jobs.PublicId exists and is unique with default (if your DB misses it)
IF COL_LENGTH('dbo.Jobs','PublicId') IS NULL
BEGIN
 ALTER TABLE dbo.Jobs ADD PublicId uniqueidentifier NULL;
 UPDATE dbo.Jobs SET PublicId = NEWSEQUENTIALID() WHERE PublicId IS NULL;
 ALTER TABLE dbo.Jobs ADD CONSTRAINT DF_Jobs_PublicId DEFAULT (NEWSEQUENTIALID()) FOR PublicId;
 ALTER TABLE dbo.Jobs ALTER COLUMN PublicId uniqueidentifier NOT NULL;
END
GO
IF NOT EXISTS (
 SELECT1 FROM sys.indexes WHERE name = 'IX_Jobs_PublicId' AND object_id = OBJECT_ID('dbo.Jobs')
)
BEGIN
 CREATE UNIQUE INDEX IX_Jobs_PublicId ON dbo.Jobs (PublicId);
END
GO

--7) Optional: Audit table for login events (use from app if desired)
IF OBJECT_ID('dbo.UserLoginAudit','U') IS NULL
BEGIN
 CREATE TABLE dbo.UserLoginAudit(
 Id int IDENTITY(1,1) PRIMARY KEY,
 UserId int NOT NULL,
 Role varchar(32) NOT NULL,
 Success bit NOT NULL,
 Ip nvarchar(45) NULL,
 LoggedAt datetime2 NOT NULL DEFAULT SYSUTCDATETIME()
 );
END
GO

PRINT 'EnableReapplyByCycle.sql completed successfully.';
