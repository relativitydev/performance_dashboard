USE Master;

declare @sql nvarchar(max) = N'declare @value nvarchar(max);
SELECT @value = convert(nvarchar(max), value)
FROM sys.fn_listextendedproperty ( ''EDDSPerformanceCreatedBy'', default, default, default, default, default, default)

IF NOT EXISTS (SELECT TOP 1 TABLE_NAME FROM [EDDSPerformance].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''RHVersion'')
AND (@value is null)
		SELECT 1;
	ELSE
		SELECT 0;
';

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'EDDSPerformance')
	SELECT 0;
ELSE
	EXEC sp_executesql @sql