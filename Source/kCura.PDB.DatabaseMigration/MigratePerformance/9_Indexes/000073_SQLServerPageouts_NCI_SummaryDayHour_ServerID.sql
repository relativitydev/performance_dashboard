USE [EDDSPerformance]
GO

DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'SQLServerPageouts') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[SQLServerPageouts]') AND name = N'NCI_SummaryDayHour_ServerID')
	BEGIN
			
		SET @SQL = 'CREATE UNIQUE NONCLUSTERED INDEX [NCI_SummaryDayHour_ServerID] ON [eddsdbo].[SQLServerPageouts]
(
	[SummaryDayHour] ASC,
	[ServerID] ASC
) WITH (IGNORE_DUP_KEY = ON)'
			
		EXEC sp_executesql @SQL
	END
END