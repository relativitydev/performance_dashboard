USE EDDSPerformance

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

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
