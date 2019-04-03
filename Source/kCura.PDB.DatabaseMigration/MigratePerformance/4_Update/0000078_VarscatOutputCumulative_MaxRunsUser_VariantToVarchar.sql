USE EDDSPerformance;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsUser'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsUser_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	ADD MaxRunsUser_tmp varchar(200) 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutputCumulative'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsUser_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutputCumulative
	SET MaxRunsUser_tmp =
		CASE
			WHEN CAST(MaxRunsUser as varchar(200)) = ''N/A'' THEN NULL
			ELSE CAST(MaxRunsUser as varchar(200))
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutputCumulative
	DROP COLUMN MaxRunsUser;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutputCumulative.MaxRunsUser_tmp', 'MaxRunsUser', 'COLUMN';
END