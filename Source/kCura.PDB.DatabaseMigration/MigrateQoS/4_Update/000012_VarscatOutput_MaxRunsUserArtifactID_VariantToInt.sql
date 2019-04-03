USE EDDSQoS;
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsUserArtifactID'
	AND DATA_TYPE = 'sql_variant')
AND NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsUserArtifactID_tmp')
BEGIN
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	ADD MaxRunsUserArtifactID_tmp INT 
END

GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = 'QoS_VarscatOutput'
	AND TABLE_SCHEMA = 'eddsdbo'
	AND COLUMN_NAME = 'MaxRunsUserArtifactID_tmp')
BEGIN
	EXEC('UPDATE eddsdbo.QoS_VarscatOutput
	SET MaxRunsUserArtifactID_tmp =
		CASE
			WHEN ISNUMERIC(CAST(MaxRunsUserArtifactID as nvarchar(MAX))) = 1 THEN CAST(MaxRunsUserArtifactID as int)
			ELSE NULL
		END;
		
	ALTER TABLE eddsdbo.QoS_VarscatOutput
	DROP COLUMN MaxRunsUserArtifactID;')
	
	EXEC sp_rename 'eddsdbo.QoS_VarscatOutput.MaxRunsUserArtifactID_tmp', 'MaxRunsUserArtifactID', 'COLUMN';
END