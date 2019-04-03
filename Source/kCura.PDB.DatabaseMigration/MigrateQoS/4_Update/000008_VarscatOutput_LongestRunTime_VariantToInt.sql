USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'LongestRunTime'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'LongestRunTime_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	ADD LongestRunTime_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'LongestRunTime_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutput
	SET LongestRunTime_tmp =
		CASE
			WHEN ISNUMERIC(CAST(LongestRunTime as nvarchar(MAX))) = 1 THEN CAST(LongestRunTime as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	DROP COLUMN LongestRunTime;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutput.LongestRunTime_tmp', 'LongestRunTime', 'COLUMN';
END