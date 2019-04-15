USE EDDSPerformance
GO

IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'NCQoS_IsActiveRunID' AND object_id = OBJECT_ID('eddsdbo.QoS_CasesToAudit'))
	EXEC sp_rename N'eddsdbo.QoS_CasesToAudit.NCQoS_IsActiveRunID', N'NCQoS_IsActive_RowID', N'INDEX';
	
IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'CiQoS_ArtifactID_DateTime' AND object_id = OBJECT_ID('eddsdbo.QoS_CasesToAudit'))
	EXEC sp_rename N'eddsdbo.QoS_CasesToAudit.CiQoS_ArtifactID_DateTime', N'CiQoS_AuditStartDate_CaseArtifactID', N'INDEX';