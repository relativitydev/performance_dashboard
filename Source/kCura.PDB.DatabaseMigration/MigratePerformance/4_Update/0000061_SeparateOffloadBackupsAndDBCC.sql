IF NOT EXISTS (SELECT TOP 1 Name FROM EDDSPerformance.eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'OffloadDBCC')
BEGIN
	INSERT INTO EDDSPerformance.eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'OffloadDBCC', 'False', '', 'When this value is set to True, the overall Best in Service score will exclude the DBCC score')
END

IF NOT EXISTS (SELECT TOP 1 Name FROM EDDSPerformance.eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'OffloadBackups')
BEGIN
	INSERT INTO EDDSPerformance.eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'OffloadBackups', 'False', '', 'When this value is set to True, the overall Best in Service score will exclude the backup score')
END

IF EXISTS (SELECT TOP 1 Name FROM EDDSPerformance.eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'PerformDBCCandBACKchecks')
BEGIN
	DECLARE @offloaded INT = (SELECT TOP 1 CAST(Value AS int)
		FROM EDDSPerformance.eddsdbo.Configuration
		WHERE Section = 'kCura.PDB' AND Name = 'PerformDBCCandBACKchecks');
	IF (@offloaded <= 0)
	BEGIN
		UPDATE EDDSPerformance.eddsdbo.Configuration
		SET Value = 'True'
		WHERE Section = 'kCura.PDB' AND Name IN ('OffloadBackups', 'OffloadDBCC');
	END
	
	DELETE FROM EDDSPerformance.eddsdbo.Configuration
	WHERE Section = 'kCura.PDB' AND Name = 'PerformDBCCandBACKchecks';
END