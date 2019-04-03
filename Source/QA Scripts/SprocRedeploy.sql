USE EDDSPerformance
GO

--This section will force a reset of key procedures in EDDSPerformance that may have been modified
DECLARE @id bigint;
 
IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[RHScriptsRun]') AND type in (N'U'))
BEGIN
	SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_BuildandRateSample.sql');
 
	UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
	SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
	WHERE id = @id;
 
	SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_QualityDataGenerator.sql');
 
	UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
	SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
	WHERE id = @id;
 
	SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_LookingGlass_3.0.1.sql');
 
	UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
	SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
	WHERE id = @id;
 
	SET @id = (SELECT MAX(id) FROM EDDSPerformance.eddsdbo.RHScriptsRun WHERE script_name = 'QoS_LookingGlass SourceDate.sql');

	UPDATE EDDSPerformance.eddsdbo.RHScriptsRun
	SET text_hash = 't2+AESuCUjPgJEqsYfFwhw=='
	WHERE id = @id;
END

--This will force you to reinstall the current version of the backup/DBCC procedures
DELETE FROM [EDDSPerformance].[eddsdbo].[Configuration]
WHERE Section = 'kCura.PDB' AND Name = 'AdminScriptsVersion';