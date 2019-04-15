USE [{{resourcedbname}}]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[DatabaseJoinedToGroup]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.DatabaseJoinedToGroup

GO

CREATE PROCEDURE [eddsdbo].[DatabaseJoinedToGroup]
	@databaseName SYSNAME,
	@availabilityGroup nvarchar(250)
WITH EXECUTE AS SELF AS
BEGIN	
	/*
	 * Determines if a given database is joined to an availabilty group or not
	 */
	DECLARE @databaseServerIsJoined bit = 

	CASE WHEN 
	EXISTS(
		SELECT database_name
		FROM sys.availability_groups
		INNER JOIN sys.availability_databases_cluster ON availability_databases_cluster.group_id = availability_groups.group_id
		WHERE sys.availability_groups.name = @availabilityGroup AND sys.availability_databases_cluster.database_name = @databaseName) 
	THEN 1 ELSE 0 END;

	SELECT @databaseServerIsJoined
END