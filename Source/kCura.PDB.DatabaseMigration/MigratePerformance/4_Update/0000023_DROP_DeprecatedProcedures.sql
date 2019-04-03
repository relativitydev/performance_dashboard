USE EDDSPerformance;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetApplicationHealthDataDaily')
	DROP PROCEDURE eddsdbo.GetApplicationHealthDataDaily;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetApplicationHealthDataHourly')
	DROP PROCEDURE eddsdbo.GetApplicationHealthDataHourly;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetRAMHealthDataDaily')
	DROP PROCEDURE eddsdbo.GetRAMHealthDataDaily;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetRAMHealthDataHourly')
	DROP PROCEDURE eddsdbo.GetRAMHealthDataHourly;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetServerDiskSummaryDataDaily')
	DROP PROCEDURE eddsdbo.GetServerDiskSummaryDataDaily;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetServerDiskSummaryDataHourly')
	DROP PROCEDURE eddsdbo.GetServerDiskSummaryDataHourly;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetServerProcessorSummaryDataDaily')
	DROP PROCEDURE eddsdbo.GetServerProcessorSummaryDataDaily;

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetServerProcessorSummaryDataHourly')
	DROP PROCEDURE eddsdbo.GetServerProcessorSummaryDataHourly;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetSQLServerSummaryDataDaily')
	DROP PROCEDURE eddsdbo.GetSQLServerSummaryDataDaily;
	
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetSQLServerSummaryDataHourly')
	DROP PROCEDURE eddsdbo.GetSQLServerSummaryDataHourly;