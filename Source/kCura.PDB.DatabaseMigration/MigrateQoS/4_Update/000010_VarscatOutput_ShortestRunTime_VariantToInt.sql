USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'ShortestRunTime'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'ShortestRunTime_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	ADD ShortestRunTime_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'ShortestRunTime_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutput
	SET ShortestRunTime_tmp =
		CASE
			WHEN ISNUMERIC(CAST(ShortestRunTime as nvarchar(MAX))) = 1 THEN CAST(ShortestRunTime as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	DROP COLUMN ShortestRunTime;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutput.ShortestRunTime_tmp', 'ShortestRunTime', 'COLUMN';
END