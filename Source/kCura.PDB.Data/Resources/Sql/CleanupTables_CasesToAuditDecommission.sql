--Expected variables
--DECLARE @batchSize int = 10000
--DECLARE @threshold datetime = '2015-08-02 21:00:00.000'
--DECLARE @maxdopLimit bit = 0

DECLARE @maxdopOption nvarchar(20)
SET @maxdopOption = CASE WHEN @maxdopLimit = 0 THEN '' ELSE 'OPTION (MAXDOP 2)' END

DECLARE @SQL nvarchar(max)
SET @SQL = 'WHILE EXISTS (SELECT TOP 1 1 FROM eddsdbo.QoS_CasesToAudit AS ca WHERE ca.ServerID NOT IN (SELECT s.ArtifactID FROM eddsdbo.[Server] AS s WHERE s.DeletedOn IS NULL AND ISNULL(s.IgnoreServer, 0) = 0 AND s.ServerTypeID = 3) AND ca.AuditStartDate >= ''' 
	+ CAST(@threshold AS NVARCHAR) + ''') BEGIN DELETE TOP (' + CAST(@batchSize AS NVARCHAR) 
	+ ') FROM eddsdbo.QoS_CasesToAudit WHERE ServerID NOT IN (SELECT s.ArtifactID FROM eddsdbo.[Server] AS s WHERE s.DeletedOn IS NULL AND ISNULL(s.IgnoreServer, 0) = 0 AND s.ServerTypeID = 3) AND AuditStartDate >= ''' 
	+ CAST(@threshold AS NVARCHAR) + ''' ' 
	+ @maxdopOption + ' END'
EXEC sp_executesql @SQL