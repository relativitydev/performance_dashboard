USE EDDSResource;
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AllDatabasesChecked]') AND type in (N'U'))
	DROP TABLE dbo.AllDatabasesChecked;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DBCCBACKKEY]') AND type in (N'U'))
	DROP TABLE dbo.DBCCBACKKEY;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[kIE_BackResults]') AND type in (N'U'))
	DROP TABLE dbo.kIE_BackResults;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[kIE_BackSummary]') AND type in (N'U'))
	DROP TABLE dbo.kIE_BackSummary;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[kIE_DBCCResults]') AND type in (N'U'))
	DROP TABLE dbo.kIE_DBCCResults;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[kIE_DBCCSummary]') AND type in (N'U'))
	DROP TABLE dbo.kIE_DBCCSummary;

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND type in (N'U'))
	DROP TABLE eddsdbo.QoS_BackResults;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_databases]') AND type in (N'U'))
	DROP TABLE eddsdbo.QoS_databases;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND type in (N'U'))
	DROP TABLE eddsdbo.QoS_DBCCResults;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBInfo]') AND type in (N'U'))
	DROP TABLE eddsdbo.QoS_DBInfo;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_tempDBCCResults]') AND type in (N'U'))
	DROP TABLE eddsdbo.QoS_tempDBCCResults;
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_tempServers]') AND type in (N'U'))
	DROP TABLE eddsdbo.QoS_tempServers;