USE [EDDSPerformance]
GO

DECLARE @enterprise bit;
DECLARE @SQL nvarchar(max);

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'QoS_CasesToAudit') 
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_CasesToAudit]') AND name = N'CiQoS_AuditStartDate_CaseArtifactID')
	BEGIN			
		SET @SQL = 'CREATE UNIQUE CLUSTERED INDEX [CiQoS_AuditStartDate_CaseArtifactID] ON [EDDSDBO].[QoS_CasesToAudit]
(
	[AuditStartDate] ASC,
	[CaseArtifactID] ASC
) WITH ( IGNORE_DUP_KEY = ON )'
			
		EXEC sp_executesql @SQL
	END
	ELSE
	BEGIN
		ALTER INDEX CiQoS_AuditStartDate_CaseArtifactID
		ON eddsdbo.QoS_CasesToAudit
		REBUILD	WITH (IGNORE_DUP_KEY = ON)
	END
END