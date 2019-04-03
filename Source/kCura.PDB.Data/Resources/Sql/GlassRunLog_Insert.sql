

--Escape apostrophes
SET @module = ISNULL(LEFT(REPLACE(@module, '''', ''''''), 100), '');
SET @taskCompleted = ISNULL(LEFT(REPLACE(@taskCompleted, '''', ''''''), 500), '');
SET @otherVars = ISNULL(LEFT(REPLACE(@otherVars, '''', ''''''), 65535), '');
SET @nextTask = ISNULL(LEFT(REPLACE(@nextTask, '''', ''''''), 500), '');

--set defaults
set @AgentID = isnull(@AgentID, -1);
set @logTimestampUtc = ISNULL(@logTimestampUtc, getutcdate());

INSERT INTO eddsdbo.QoS_GlassRunLog(AgentID, LogTimestampUTC, Module, TaskCompleted, OtherVars, NextTask, LogLevel) 
VALUES (
	@AgentID
	,@logTimestampUtc
	,@module
	,@taskCompleted
	,@otherVars
	,@nextTask
	,@logLevel)
	
SELECT @@IDENTITY