USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_DBCCResults]') AND type in (N'U'))
BEGIN
	DELETE BR
	FROM eddsdbo.QoS_DBCCResults BR
	LEFT OUTER JOIN (
	   SELECT MIN(kdbccbID) as RowId, CaseArtifactID, DBName, LoggedDate 
	   FROM eddsdbo.QoS_DBCCResults 
	   GROUP BY CaseArtifactID, DBName, LoggedDate
	) as KeepRows ON
	   BR.kdbccbID = KeepRows.RowId
	WHERE
	   KeepRows.RowId IS NULL;
END