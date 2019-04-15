USE EDDSPerformance
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserExperienceServerSummary' AND TABLE_SCHEMA = 'eddsdbo') 
BEGIN
	TRUNCATE TABLE eddsdbo.QoS_UserExperienceServerSummary;
	
	IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_SampleHistory' AND TABLE_SCHEMA = 'eddsdbo')
	AND EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = 'eddsdbo')
	AND EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_UserXInstanceSummary' AND TABLE_SCHEMA = 'eddsdbo')
	BEGIN
		WITH perWorkspaceLRQs AS
		(
			SELECT
				S.ArtifactID AS ServerArtifactID,
				MIN(VODC.ServerName) AS [Server],
				VODC.CaseArtifactID,
				MIN(ISNULL(C.Name, 'Deleted')) AS Workspace,
				VODC.SummaryDayHour,
				VODC.QoSHourID,
				COUNT(DISTINCT VODC.UserID) TotalUsers,
				AVG(ISNULL(UX.Score, 100)) Score,
				SUM(CASE WHEN VODC.IsLongRunning = 1 THEN 1 ELSE 0 END) TotalLongRunning,
				SUM(CASE WHEN QoSAction IN (281, 282) THEN 1 ELSE 0 END) TotalSearchAudits,
				SUM(CAST(VODC.ExecutionTime as bigint)) TotalExecutionTime,
				COUNT(VODC.VODCID) TotalAudits,
				SUM(CASE WHEN QoSAction IN (281, 282) AND VODC.IsComplex = 0 AND VODC.IsLongRunning = 1 THEN 1 ELSE 0 END) NumberOfLRSQ,
				SUM(CASE WHEN QoSAction IN (281, 282) AND VODC.IsComplex = 0 AND VODC.IsLongRunning = 0 THEN 1 ELSE 0 END) NumberOfNLRSQ
			FROM EDDSDBO.QoS_VarscatOutputDetailCumulative VODC WITH(NOLOCK)
			INNER JOIN eddsdbo.QoS_SampleHistory SH WITH(NOLOCK)
				ON VODC.QoSHourID = SH.QoSHourID
			INNER JOIN eddsdbo.[Server] S WITH(NOLOCK)
				ON VODC.ServerName = S.ServerName
			CROSS APPLY (
				SELECT AVG(ISNULL(AvgSQScorePerUser, 100)) Score
				FROM eddsdbo.QoS_UserXInstanceSummary WITH(NOLOCK)
				WHERE QoSHourID = VODC.QoSHourID
			) UX (Score)
			LEFT JOIN EDDS.EDDSDBO.[Case] C WITH(NOLOCK)
				ON VODC.CaseArtifactID = C.ArtifactID
			WHERE SH.IsActiveSample = 1
			AND S.ServerTypeID = 3
			AND (S.IgnoreServer IS NULL OR S.IgnoreServer = 0)
			AND S.DeletedOn IS NULL
			AND S.ArtifactID IS NOT NULL
			AND NOT EXISTS (SELECT TOP 1 1 FROM eddsdbo.QoS_UserExperienceServerSummary WHERE QoSHourID = SH.QoSHourID)
			GROUP BY VODC.SummaryDayHour, VODC.QoSHourID, S.ArtifactID, VODC.CaseArtifactID
			)
		INSERT INTO eddsdbo.QoS_UserExperienceServerSummary
			(ServerArtifactID, [Server], CaseArtifactID, Workspace, Score, TotalLongRunning, TotalUsers,
			TotalSearchAudits, TotalNonSearchAudits, TotalAudits, TotalExecutionTime, SummaryDayHour, QoSHourID)
		SELECT
			[ServerArtifactID],
			[Server],
			CaseArtifactID,
			Workspace,
			Score,
			TotalLongRunning,
			TotalUsers,
			TotalSearchAudits,
			TotalAudits - TotalSearchAudits AS TotalNonSearchAudits,
			TotalAudits,
			TotalExecutionTime,
			SummaryDayHour,
			QoSHourID
		FROM perWorkspaceLRQs
	END
END