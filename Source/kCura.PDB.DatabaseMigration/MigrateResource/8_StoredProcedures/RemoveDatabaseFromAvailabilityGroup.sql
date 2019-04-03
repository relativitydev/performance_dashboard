USE [{{resourcedbname}}]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[RemoveDatabaseFromAvailabilityGroup]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.RemoveDatabaseFromAvailabilityGroup

GO

CREATE PROCEDURE [eddsdbo].[RemoveDatabaseFromAvailabilityGroup]
	@databaseName SYSNAME,
	@availabilityGroup nvarchar(250)
WITH EXECUTE AS SELF AS
BEGIN
	/* 
	 * Removes a database from the given availability group
	 */	 
	--Suspend database mirroring
	DECLARE @sql nvarchar(1000) = 'USE [master] ALTER DATABASE '+ QUOTENAME(@databaseName)+ ' SET HADR SUSPEND'
	EXEC sp_executesql @sql

	--Remove database from availability group
	SET @sql = 'USE [master] ALTER AVAILABILITY GROUP '+ QUOTENAME(@availabilityGroup) + ' REMOVE DATABASE ' + QUOTENAME(@databaseName)
	EXEC sp_executesql @sql
END