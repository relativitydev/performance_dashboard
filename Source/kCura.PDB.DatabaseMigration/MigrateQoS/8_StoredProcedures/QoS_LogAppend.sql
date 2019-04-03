USE [EDDSQoS]
GO

/****** Object:  StoredProcedure [eddsdbo].[QoS_LogAppend]    Script Date: 9/4/2014 1:39:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE EDDSQoS
GO
IF EXISTS (select 1 from sysobjects where [name] = 'QoS_LogAppend' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.QoS_LogAppend
END
GO
CREATE PROCEDURE eddsdbo.QoS_LogAppend
	@AgentID INT = -1, --This is the ArtifactID of the agent calling the procedure
	@module varchar(100), --The name of the calling stored procedure.
	@taskCompleted varchar(max),
	@otherVars varchar(max) = null,
	@nextTask varchar(500) = null, --This is the next task that will run
	@loglevel int = null,
	@eddsPerformanceServerName nvarchar(255)
AS
BEGIN

DECLARE @SQL nvarchar(max);

--Escape apostrophes
SET @module = ISNULL(REPLACE(@module, '''', ''''''), '');
SET @taskCompleted = ISNULL(REPLACE(@taskCompleted, '''', ''''''), '');
SET @otherVars = ISNULL(REPLACE(@otherVars, '''', ''''''), '');
SET @nextTask = ISNULL(REPLACE(@nextTask, '''', ''''''), '');
--set defaults
set @AgentID = isnull(@AgentID, -1);

SET @SQL = 'INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_GlassRunLog	
	(AgentID, LogTimestampUTC, Module, TaskCompleted, OtherVars, LogLevel, NextTask) VALUES 
	(' + CAST(@AgentID as nvarchar) + ', '
	+ '''' + CONVERT(varchar(50), GETUTCDATE(), 121) + ''', ' --log timestamp - when the log entry was made
	+ '''' + @module + ''', '
	+ '''' + @taskCompleted + ''', '
	+ '''' + @otherVars + ''', '
	+ ISNULL(CAST(@loglevel AS nvarchar), 'NULL') + ', '
	+ '''' + @nextTask + ''')'

EXEC sp_executesql @SQL
END