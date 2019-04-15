USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'TotalLRQRunTime'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'TotalLRQRunTime_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	ADD TotalLRQRunTime_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'TotalLRQRunTime_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutputCumulative
	SET TotalLRQRunTime_tmp =
		CASE
			WHEN ISNUMERIC(CAST(TotalLRQRunTime as nvarchar(MAX))) = 1 THEN CAST(TotalLRQRunTime as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	DROP COLUMN TotalLRQRunTime;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutputCumulative.TotalLRQRunTime_tmp', 'TotalLRQRunTime', 'COLUMN';
END