USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_CasesToAudit'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'WorkspaceName'
	AND CHARACTER_MAXIMUM_LENGTH > 50)
BEGIN
	--There shouldn't be any rows that satisfy this condition, but just in case...
	UPDATE eddsdbo.QoS_CasesToAudit
	SET WorkspaceName = SUBSTRING(WorkspaceName, 1, 50)
	WHERE LEN(WorkspaceName) > 50

	--Resize WorkspaceName column
	ALTER TABLE eddsdbo.QoS_CasesToAudit
	ALTER COLUMN WorkspaceName varchar(50)
END