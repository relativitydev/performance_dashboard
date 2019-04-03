USE [EDDSPerformance]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE EDDSPerformance
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
	@loglevel int = null
AS
BEGIN

	--Escape apostrophes
	SET @module = ISNULL(REPLACE(@module, '''', ''''''), '');
	SET @taskCompleted = ISNULL(REPLACE(@taskCompleted, '''', ''''''), '');
	SET @otherVars = ISNULL(REPLACE(@otherVars, '''', ''''''), '');
	SET @nextTask = ISNULL(REPLACE(@nextTask, '''', ''''''), '');
	--set defaults
	set @AgentID = isnull(@AgentID, -1);

	INSERT INTO EDDSPerformance.eddsdbo.QoS_GlassRunLog	
		(AgentID, LogTimestampUTC, Module, TaskCompleted, OtherVars, NextTask, LogLevel) VALUES 
		(@AgentID, GETUTCDATE(), @module, @taskCompleted, @otherVars, @nextTask, @loglevel)

END