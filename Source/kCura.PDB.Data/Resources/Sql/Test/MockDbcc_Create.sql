-- EDDSPerformance

INSERT INTO eddsdbo.[MockDbccServerResults] (
	[Server],
	[Database],
	[CaseArtifactID],
	[LastCleanDBCCDate]
) VALUES (
	@server,
	@database,
	@caseArtifactID,
	@lastCleanDBCCDate)