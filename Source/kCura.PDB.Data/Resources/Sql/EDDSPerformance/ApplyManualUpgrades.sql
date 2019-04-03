USE EDDSPerformance;

SET IDENTITY_INSERT [eddsdbo].[ServerType] ON
IF NOT EXISTS(SELECT * from [eddsdbo].[ServerType] WHERE ServerTypeID=11)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (11, N'WebAPI', NULL)
IF NOT EXISTS(SELECT * from [eddsdbo].[ServerType] WHERE ServerTypeID=12)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (12, N'Services', NULL)
IF NOT EXISTS(SELECT * from [eddsdbo].[ServerType] WHERE ServerTypeID=99)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (99, N'Unrecognized', NULL)
IF NOT EXISTS(SELECT * from [eddsdbo].[ServerType] WHERE ServerTypeID=20)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (20, N'WebBackground', NULL)
IF NOT EXISTS(SELECT * from [eddsdbo].[ServerType] WHERE ServerTypeID=21)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (21, N'Processing', NULL)
IF NOT EXISTS(SELECT * from [eddsdbo].[ServerType] WHERE ServerTypeID=22)
INSERT INTO [eddsdbo].[ServerType] ([ServerTypeID], [ServerTypeName], [ArtifactID]) VALUES (22, N'Analytics', NULL)
SET IDENTITY_INSERT [eddsdbo].[ServerType] OFF

GO

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

--Add the new IgnoreServer field to the Server table if it doesn't already exist
IF COL_LENGTH('eddsdbo.Server','IgnoreServer') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.Server ADD IgnoreServer bit NULL DEFAULT(0)
	END
GO
	
--Update any existing null entries to zero (false)
UPDATE eddsdbo.Server SET IgnoreServer = 0 WHERE IgnoreServer is null

GO

--Add the new DriverLetter field to the ServerDiskDW table if it doesn't already exist
IF COL_LENGTH('eddsdbo.ServerDiskDW','DriveLetter') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.ServerDiskDW ADD [DriveLetter] [NVARCHAR](2) NULL
	END
GO

--Add the new DriverLetter field to the ServerDiskSummary table if it doesn't already exist
IF COL_LENGTH('eddsdbo.ServerDiskSummary','DriveLetter') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.ServerDiskSummary ADD [DriveLetter] [NVARCHAR](2) NULL
	END
GO

USE EDDSPerformance

BEGIN TRANSACTION

IF COL_LENGTH('eddsdbo.Server','ResponsibleAgent') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.Server ADD
	ResponsibleAgent nvarchar(MAX) NULL
	END

ALTER TABLE eddsdbo.Server SET (LOCK_ESCALATION = TABLE)
COMMIT

GO

--Add the new TotalNRQCount field to the LRQCountDW table if it doesn't already exist
IF COL_LENGTH('eddsdbo.LRQCountDW','TotalNRQCount') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.LRQCountDW ADD [TotalNRQCount] [int] NULL
	END
GO

--Add the new TotalNRQCount field to the BISSummary table if it doesn't already exist
IF COL_LENGTH('eddsdbo.BISSummary','TotalNRQCount') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.BISSummary ADD [TotalNRQCount] [int] NULL
	END
GO

--Add the new TotalNRQCount field to the PerformanceSummary table if it doesn't already exist
IF COL_LENGTH('eddsdbo.PerformanceSummary','TotalNRQCount') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.PerformanceSummary ADD [TotalNRQCount] [int] NULL
	END
GO

IF COL_LENGTH('eddsdbo.BISSummary','DocumentCount') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.BISSummary ADD [DocumentCount] [bigint] NULL
	END
GO
IF COL_LENGTH('eddsdbo.LRQCountDW','totalQtime') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.LRQCountDW ADD [totalQtime] [int] NULL
	END
GO

UPDATE EDDSPerformance.eddsdbo.Configuration
SET Value =  
	CAST(
ISNULL((SELECT ag.ArtifactID
FROM EDDS.eddsdbo.Agent ag
JOIN EDDS.eddsdbo.ResourceServer RS
ON rs.ArtifactID = ag.ServerArtifactID
WHERE ag.Name = 'Performance Dashboard Agent (1)'
), '') as nVarchar(7))
WHERE Name = 'RollupAgent'

GO

DECLARE @PDBAgentOrig NVARCHAR(255)

SELECT @PDBAgentOrig = Value FROM EDDSPerformance.eddsdbo.Configuration
WHERE Name = 'RollupAgent'

UPDATE EDDSPerformance.eddsdbo.Server
SET ResponsibleAgent = @PDBAgentOrig

GO

ALTER TABLE EDDSPerformance.eddsdbo.ServerDiskDW
ALTER COLUMN DriveLetter nvarchar(300)

GO

ALTER TABLE EDDSPerformance.eddsdbo.ServerDiskSummary
ALTER COLUMN DriveLetter nvarchar(300)

GO

--[LRQCountDW] index
USE [EDDSPerformance]
GO

/****** Object:  Index [kie_measureDate]    Script Date: 10/04/2013 11:09:44 ******/
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[LRQCountDW]') AND name = N'kIE_MeasureDate')
DROP INDEX [kie_measureDate] ON [eddsdbo].[LRQCountDW] WITH ( ONLINE = OFF )
GO

--[BISSummary] index
USE [EDDSPerformance]
GO

/****** Object:  Index [KIE_measuredate]    Script Date: 10/04/2013 11:09:18 ******/
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[BISSummary]') AND name = N'kIE_MeasureDate')
DROP INDEX [kIE_MeasureDate] ON [eddsdbo].[BISSummary] WITH ( ONLINE = OFF )