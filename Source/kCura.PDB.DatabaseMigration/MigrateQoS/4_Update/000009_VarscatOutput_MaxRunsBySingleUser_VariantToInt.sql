USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsBySingleUser'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsBySingleUser_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	ADD MaxRunsBySingleUser_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsBySingleUser_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutput
	SET MaxRunsBySingleUser_tmp =
		CASE
			WHEN ISNUMERIC(CAST(MaxRunsBySingleUser as nvarchar(MAX))) = 1 THEN CAST(MaxRunsBySingleUser as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	DROP COLUMN MaxRunsBySingleUser;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutput.MaxRunsBySingleUser_tmp', 'MaxRunsBySingleUser', 'COLUMN';
END