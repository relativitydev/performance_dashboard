USE EDDSQoS

;WITH simpleQueries AS
(
	SELECT
		UserID,
		vod.SearchArtifactID,
		CASE
			WHEN SUM(ISNULL(ExecutionTime, 0)) > 2000 THEN 0
			ELSE 100
		END SearchScore
	FROM eddsdbo.QoS_VarscatOutputDetail vod WITH(NOLOCK)
	WHERE QoSAction IN (281, 282, 283)
		AND IsComplex = 0
		AND [Timestamp] >= DATEADD(HH, DATEDIFF(hh, 0, getUTCdate()) - 1, 0)
		AND [Timestamp] < DATEADD(HH, DATEDIFF(hh, 0, getUTCdate()), 0)
	GROUP BY UserID, SearchArtifactID, ISNULL(CAST(vod.QueryID as varchar(36)), vod.VODID)
),
userSearchScores AS
(
	SELECT UserID, AVG(SearchScore) UserScore
	FROM simpleQueries
	GROUP BY UserID
)
SELECT CAST(ISNULL(AVG(UserScore), 100) as int) AvgUserScore
FROM userSearchScores;