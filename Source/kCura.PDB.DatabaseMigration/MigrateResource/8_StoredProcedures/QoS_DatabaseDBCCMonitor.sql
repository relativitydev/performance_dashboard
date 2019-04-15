USE [{{resourcedbname}}]
GO

/****** Object:  StoredProcedure [dbo].[kIE_BackupAndDBCCCheckServerMon]    Script Date: 05/05/2014 12:56:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DatabaseDBCCMonitor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[QoS_DatabaseDBCCMonitor]
GO

CREATE PROCEDURE [eddsdbo].[QoS_DatabaseDBCCMonitor]
	@tempDBName NVARCHAR(255),
	@currentServerName NVARCHAR(255)
WITH EXEC AS SELF
AS
BEGIN
	declare @sql nvarchar(250) = 'DBCC DBINFO(' + QUOTENAME(@tempDBName) + ') WITH TABLERESULTS, NO_INFOMSGS';

	INSERT INTO EDDSQoS.EDDSDBO.QoS_DBInfo
	EXECUTE(@sql)
	
	-- Pull out object for last known good dbcc checkDB from EDDSDBO.QoS_DBInfo temp table
	-- Insert the values into the EDDSDBO.QoS_DBCCResults table
	INSERT INTO EDDSQoS.EDDSDBO.QoS_DBCCServerResults (ServerName, DBName, CaseArtifactID, LastCleanDBCCDate)
	SELECT TOP 1
		@currentServerName,
		@tempDBName,
		CASE WHEN ISNUMERIC(REPLACE(@tempDBName, 'EDDS', '')) = 1 THEN CAST(REPLACE(@tempDBName, 'EDDS', '') AS int)
			WHEN @tempDBName = 'EDDS' THEN -1
			ELSE -2
		END,
		Value
	FROM EDDSQoS.EDDSDBO.QoS_DBInfo WHERE Field = 'dbi_dbccLastKnownGood'
END