USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkSysOutput' AND TABLE_SCHEMA = 'eddsdbo')
BEGIN
	alter table [eddsdbo].[TuningForkSysOutput] 
	alter column [Value] int null

	alter table [eddsdbo].[TuningForkSysOutput] 
	alter column [Value_in_use] int null

	alter table [eddsdbo].[TuningForkSysOutput] 
	alter column [kIE_value] int null

	alter table [eddsdbo].[TuningForkSysOutput] 
	alter column [Is_Dynamic] bit null
END