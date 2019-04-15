EXECUTE dbo.DatabaseIntegrityCheck
	@Databases = 'EDDS%',
	@LogToTable = 'Y',
	@PhysicalOnly = 'Y'