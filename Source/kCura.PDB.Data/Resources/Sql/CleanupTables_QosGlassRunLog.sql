--Expected variables
--DECLARE @batchSize int = 10000
--DECLARE @threshold datetime = '2015-08-02 21:00:00.000'
--DECLARE @tableScope nvarchar(120) = 'EDDSDBO.QoS_GlassRunLog'
--DECLARE @dateTimeColumn nvarchar(64) = 'LogTimestampUTC'
--DECLARE @maxdopLimit bit = 0

DECLARE @maxdopOption nvarchar(20)
SET @maxdopOption = CASE WHEN @maxdopLimit = 0 THEN '' ELSE 'OPTION (MAXDOP 2)' END



--- Delete from EventLogs
--- Delete from QoS_GlassRunLog

DECLARE @SQL nvarchar(max)
-- While exists
SET @SQL = 'DECLARE @logIds table (id int);'
	+ ' WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.QoS_GlassRunLog WHERE LogTimestampUTC < ''' + CAST(@threshold AS NVARCHAR) 
	+ ''') BEGIN'
	+ ' BEGIN TRY'
--- Query the Ids to delete
	+ ' INSERT INTO @logIds SELECT TOP ('  + CAST(@batchSize AS NVARCHAR) + ') GRLogID FROM eddsdbo.QoS_GlassRunLog WHERE LogTimestampUTC < ''' 
	+ CAST(@threshold AS NVARCHAR) + ''' ' + @maxdopOption 
-- DELETE THEM from EventLogs
	+ ' DELETE el FROM eddsdbo.EventLogs el INNER JOIN @logIds l ON el.LogId = l.id '  + @maxdopOption
-- DELETE THEM from QoS_GlassRunLog
	+ ' DELETE gl FROM eddsdbo.QoS_GlassRunLog gl INNER JOIN @logIds l on gl.GRLogId = l.id ' + @maxdopOption
	+ ' END TRY BEGIN CATCH RAISERROR(''Failed to delete from EventLogs or QoS_GlassRunLog dynamically based on datetime [' 
	+ CAST(@threshold AS NVARCHAR) + ']'', 20, -1) with log END CATCH END'
EXEC sp_executesql @SQL