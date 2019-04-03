USE EDDSPerformance

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

--Add the new DocumentCount field to the BISSummary table if it doesn't already exist
IF COL_LENGTH('eddsdbo.LRQCountDW','totalQtime') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.LRQCountDW ADD [totalQtime] [int] NULL
	END
GO

