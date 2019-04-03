--Expected variables
--DECLARE @batchSize int = 10000
--DECLARE @threshold bigint = '123456'
--DECLARE @tableScope nvarchar(120) = 'EDDSDBO.QoS_GlassRunLog'
--DECLARE @maxdopLimit bit = 0

DECLARE @maxdopOption nvarchar(20)
SET @maxdopOption = CASE WHEN @maxdopLimit = 0 THEN '' ELSE 'OPTION (MAXDOP 2)' END

DECLARE @SQL nvarchar(max)
SET @SQL = 'WHILE EXISTS (SELECT TOP 1 1 FROM ' 
	+ @tableScope + ' WHERE QoSHourID < ''' + CAST(@threshold AS NVARCHAR) 
	+ ''') BEGIN BEGIN TRY DELETE TOP (' + CAST(@batchSize AS NVARCHAR) + ') FROM ' + @tableScope 
	+ ' WHERE QoSHourID < ''' + CAST(@threshold AS NVARCHAR) + ''' ' 
	+ @maxdopOption + ' END TRY BEGIN CATCH RAISERROR(''Failed to delete from table ' + @tableScope + ' dynamically based on datetime [' 
	+ CAST(@threshold AS NVARCHAR) + ']'', 20, -1) with log END CATCH END'
EXEC sp_executesql @SQL