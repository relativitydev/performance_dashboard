USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_SourceDatetime') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_SourceDatetime]') AND name = N'NCQoS_Quotidian')
	BEGIN			
		SET @SQL = 'CREATE UNIQUE NONCLUSTERED INDEX [NCQoS_Quotidian] ON [eddsdbo].[QoS_SourceDatetime]
	(
		[quotidian] ASC
	) WITH (IGNORE_DUP_KEY = ON)'
			
		EXEC sp_executesql @SQL
	END
END