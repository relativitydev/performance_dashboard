USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'TotalLRQRunTime'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'TotalLRQRunTime_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	ADD TotalLRQRunTime_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'TotalLRQRunTime_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutput
	SET TotalLRQRunTime_tmp =
		CASE
			WHEN ISNUMERIC(CAST(TotalLRQRunTime as nvarchar(MAX))) = 1 THEN CAST(TotalLRQRunTime as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	DROP COLUMN TotalLRQRunTime;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutput.TotalLRQRunTime_tmp', 'TotalLRQRunTime', 'COLUMN';
END