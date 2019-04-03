USE EDDSPerformance

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

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