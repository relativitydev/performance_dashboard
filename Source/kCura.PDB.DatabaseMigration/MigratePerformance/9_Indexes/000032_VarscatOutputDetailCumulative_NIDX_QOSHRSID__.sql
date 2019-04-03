USE [EDDSPerformance]
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_VarscatOutputDetailCumulative') 
BEGIN
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'NIDX_QOSHRSID')
	BEGIN
		DROP INDEX NIDX_QOSHRSID ON eddsdbo.QoS_VarscatOutputDetailCumulative
	END
END

GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_VarscatOutputDetailCumulative') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'NIDX_QOSHRSID')
	BEGIN
		IF (SELECT @@VERSION) NOT LIKE '%Enterprise%'
			SET @enterprise = 0;
		ELSE
			SET @enterprise = 1;
			
		SET @SQL = 'CREATE NONCLUSTERED INDEX [NIDX_QOSHRSID] ON [eddsdbo].[QoS_VarscatOutputDetailCumulative]
(
	[QoSHourID] ASC,
	[SearchArtifactID] ASC
)
INCLUDE ([ServerName],
	[SummaryDayHour],
	[CaseArtifactID],
	[UserID],
	[ExecutionTime],
	[QoSAction],
	[IsLongRunning],
	[IsComplex],
	[AuditID],
	[SearchName],
	[QueryID])'
			
		EXEC sp_executesql @SQL
	END
END