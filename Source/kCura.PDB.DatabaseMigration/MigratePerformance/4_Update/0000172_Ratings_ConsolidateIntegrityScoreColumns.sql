USE EDDSPerformance
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings')
BEGIN
	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'IntegrityScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		ADD IntegrityScore DECIMAL (5,2) NULL
		CONSTRAINT DF_Ratings_IntegrityScore DEFAULT 100
	END

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'WeekIntegrityScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		ADD WeekIntegrityScore DECIMAL (5,2) NULL
		CONSTRAINT DF_Ratings_WeekIntegrityScore DEFAULT 100
	END

	IF EXISTS (
		SELECT
			default_constraints.name
		FROM 
			sys.all_columns

				INNER JOIN
			sys.tables
				ON all_columns.object_id = tables.object_id

				INNER JOIN 
			sys.schemas
				ON tables.schema_id = schemas.schema_id

				INNER JOIN
			sys.default_constraints
				ON all_columns.default_object_id = default_constraints.object_id

		WHERE schemas.name = 'eddsdbo'
			AND tables.name = 'QoS_Ratings'
			AND all_columns.name = 'BackupScore'
			AND default_constraints.name = 'DF_Ratings_BackupScore'
	)
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP CONSTRAINT DF_Ratings_BackupScore
	END

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'BackupScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP COLUMN BackupScore
	END

	IF EXISTS (
		SELECT
			default_constraints.name
		FROM 
			sys.all_columns

				INNER JOIN
			sys.tables
				ON all_columns.object_id = tables.object_id

				INNER JOIN 
			sys.schemas
				ON tables.schema_id = schemas.schema_id

				INNER JOIN
			sys.default_constraints
				ON all_columns.default_object_id = default_constraints.object_id

		WHERE schemas.name = 'eddsdbo'
			AND tables.name = 'QoS_Ratings'
			AND all_columns.name = 'DBCCScore'
			AND default_constraints.name = 'DF_Ratings_DBCCScore'
	)
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP CONSTRAINT DF_Ratings_DBCCScore
	END

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'DBCCScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP COLUMN DBCCScore
	END

	IF EXISTS (
		SELECT
			default_constraints.name
		FROM 
			sys.all_columns

				INNER JOIN
			sys.tables
				ON all_columns.object_id = tables.object_id

				INNER JOIN 
			sys.schemas
				ON tables.schema_id = schemas.schema_id

				INNER JOIN
			sys.default_constraints
				ON all_columns.default_object_id = default_constraints.object_id

		WHERE schemas.name = 'eddsdbo'
			AND tables.name = 'QoS_Ratings'
			AND all_columns.name = 'WeekBackupFrequencyScore'
			AND default_constraints.name = 'DF_Ratings_WeekBackupFrequencyScore'
	)
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP CONSTRAINT DF_Ratings_WeekBackupFrequencyScore
	END

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'WeekBackupFrequencyScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP COLUMN WeekBackupFrequencyScore
	END

	IF EXISTS (
		SELECT
			default_constraints.name
		FROM 
			sys.all_columns

				INNER JOIN
			sys.tables
				ON all_columns.object_id = tables.object_id

				INNER JOIN 
			sys.schemas
				ON tables.schema_id = schemas.schema_id

				INNER JOIN
			sys.default_constraints
				ON all_columns.default_object_id = default_constraints.object_id

		WHERE schemas.name = 'eddsdbo'
			AND tables.name = 'QoS_Ratings'
			AND all_columns.name = 'WeekBackupCoverageScore'
			AND default_constraints.name = 'DF_Ratings_WeekBackupCoverageScore'
	)
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP CONSTRAINT DF_Ratings_WeekBackupCoverageScore
	END

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'WeekBackupCoverageScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP COLUMN WeekBackupCoverageScore
	END

	IF EXISTS (
		SELECT
			default_constraints.name
		FROM 
			sys.all_columns

				INNER JOIN
			sys.tables
				ON all_columns.object_id = tables.object_id

				INNER JOIN 
			sys.schemas
				ON tables.schema_id = schemas.schema_id

				INNER JOIN
			sys.default_constraints
				ON all_columns.default_object_id = default_constraints.object_id

		WHERE schemas.name = 'eddsdbo'
			AND tables.name = 'QoS_Ratings'
			AND all_columns.name = 'WeekDBCCFrequencyScore'
			AND default_constraints.name = 'DF_Ratings_WeekDBCCFrequencyScore'
	)
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP CONSTRAINT DF_Ratings_WeekDBCCFrequencyScore
	END

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'WeekDBCCFrequencyScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP COLUMN WeekDBCCFrequencyScore
	END

	IF EXISTS (
		SELECT
			default_constraints.name
		FROM 
			sys.all_columns

				INNER JOIN
			sys.tables
				ON all_columns.object_id = tables.object_id

				INNER JOIN 
			sys.schemas
				ON tables.schema_id = schemas.schema_id

				INNER JOIN
			sys.default_constraints
				ON all_columns.default_object_id = default_constraints.object_id

		WHERE schemas.name = 'eddsdbo'
			AND tables.name = 'QoS_Ratings'
			AND all_columns.name = 'WeekDBCCCoverageScore'
			AND default_constraints.name = 'DF_Ratings_WeekDBCCCoverageScore'
	)
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP CONSTRAINT DF_Ratings_WeekDBCCCoverageScore
	END

	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_Ratings' AND COLUMN_NAME = 'WeekDBCCCoverageScore')
	BEGIN
		ALTER TABLE eddsdbo.QoS_Ratings
		DROP COLUMN WeekDBCCCoverageScore
	END
END