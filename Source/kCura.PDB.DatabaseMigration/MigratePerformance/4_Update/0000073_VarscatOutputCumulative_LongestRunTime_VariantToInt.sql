USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'LongestRunTime'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'LongestRunTime_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	ADD LongestRunTime_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'LongestRunTime_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutputCumulative
	SET LongestRunTime_tmp =
		CASE
			WHEN ISNUMERIC(CAST(LongestRunTime as nvarchar(MAX))) = 1 THEN CAST(LongestRunTime as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	DROP COLUMN LongestRunTime;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutputCumulative.LongestRunTime_tmp', 'LongestRunTime', 'COLUMN';
END