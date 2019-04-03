USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackResults]') AND type in (N'U'))
BEGIN
	DELETE BR
	FROM eddsdbo.QoS_BackResults BR
	LEFT OUTER JOIN (
	   SELECT MIN(kdbbuID) as RowId, CaseArtifactID, DBName, LoggedDate 
	   FROM eddsdbo.QoS_BackResults 
	   GROUP BY CaseArtifactID, DBName, LoggedDate
	) as KeepRows ON
	   BR.kdbbuID = KeepRows.RowId
	WHERE
	   KeepRows.RowId IS NULL;
END