USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_VarscatOutputDetailCumulative') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'IX_HourAction')
	BEGIN			
		SET @SQL = 'CREATE NONCLUSTERED INDEX IX_HourAction ON [eddsdbo].[QoS_VarscatOutputDetailCumulative]
			(
				[QoSHourID],
				[QoSAction]
			)
			INCLUDE ([SummaryDayHour],[CaseArtifactID],[SearchArtifactID],[SearchName],[AuditID],[UserID],[ExecutionTime],[IsLongRunning],[IsComplex],[ServerName])'
			
		EXEC sp_executesql @SQL
	END
END