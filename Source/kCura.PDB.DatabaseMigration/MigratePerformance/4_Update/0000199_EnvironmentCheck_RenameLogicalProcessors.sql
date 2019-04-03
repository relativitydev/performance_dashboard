USE EDDSPerformance
GO

IF EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EnvironmentCheckServerDetails' AND COLUMN_NAME = 'LocalProcessors')
BEGIN
	EXEC sp_rename 'eddsdbo.EnvironmentCheckServerDetails.LocalProcessors', 'LogicalProcessors', 'COLUMN'
END
