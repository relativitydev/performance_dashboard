USE EDDSPerformance;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetBISHealthData')
	DROP PROCEDURE eddsdbo.GetBISHealthData;

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'UpdateBISScores')
	DROP PROCEDURE eddsdbo.UpdateBISScores;

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'LoadBISSummary')
	DROP PROCEDURE eddsdbo.LoadBISSummary;