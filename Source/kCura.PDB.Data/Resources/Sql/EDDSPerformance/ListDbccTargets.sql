--EDDSPerformance
SELECT
	[DbccTargetId],
	[ServerName],
	[DatabaseName],
	[Active]
FROM [eddsdbo].[DBCCTarget] WITH(NOLOCK)