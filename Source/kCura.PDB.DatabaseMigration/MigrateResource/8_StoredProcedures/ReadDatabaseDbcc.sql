USE [{{resourcedbname}}]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[ReadDatabaseDbcc]') AND type in (N'P', N'PC'))
DROP PROCEDURE [eddsdbo].[ReadDatabaseDbcc]
GO

CREATE PROCEDURE [eddsdbo].[ReadDatabaseDbcc]
	@databaseName NVARCHAR(255)
WITH EXEC AS SELF
AS
BEGIN
	truncate table [EDDSQoS].[eddsdbo].[DbccInfoResults]
	
	INSERT INTO [EDDSQoS].[eddsdbo].[DbccInfoResults]
	EXECUTE('DBCC DBINFO([' + @databaseName + ']) WITH TABLERESULTS, NO_INFOMSGS') 
	
	-- Pull out value for last known good dbcc checkDB from EDDSDBO.DbccInfoResults temp table
	SELECT TOP 1 [Value]
	FROM [EDDSQoS].EDDSDBO.DbccInfoResults WHERE Field = 'dbi_dbccLastKnownGood'
END