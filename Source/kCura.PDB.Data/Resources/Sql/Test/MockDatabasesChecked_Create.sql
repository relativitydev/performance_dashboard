-- EDDSPerformance

INSERT INTO eddsdbo.[MockDatabasesChecked] (
	[Server],
	[Database],
	[CreatedOn]
) VALUES (
	@server,
	@database,
	@createdOn)