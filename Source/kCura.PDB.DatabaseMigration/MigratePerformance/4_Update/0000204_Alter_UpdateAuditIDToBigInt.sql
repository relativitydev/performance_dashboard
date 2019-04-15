USE [EDDSPerformance]
GO

/*
QoS_UserExperienceSearchSummary (EDDSPerformance)
QoS_VarscatOutputDetailCumulative (EDDSPerformance)
QoS_VarscatOutputDetail (EDDSQoS)
VarscatOutputDetail (workspace DB)
QoS_SearchAuditRows (workspace DB)
QoS_SearchAuditParsed (workspace DB)
*/

IF exists(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_UserExperienceSearchSummary' AND COLUMN_NAME = 'LastAuditID' AND DATA_TYPE = 'INT')
BEGIN
    ALTER TABLE eddsdbo.QoS_UserExperienceSearchSummary ALTER COLUMN LastAuditID bigint
END

IF exists(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND COLUMN_NAME = 'AuditID' AND DATA_TYPE = 'INT')
BEGIN
    
	---drop indexes
	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'NIDX_QOSHRSID')
	BEGIN
		DROP INDEX NIDX_QOSHRSID ON eddsdbo.QoS_VarscatOutputDetailCumulative
	END

	IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = N'IX_HourAction')
	BEGIN
		DROP INDEX [IX_HourAction] ON [eddsdbo].[QoS_VarscatOutputDetailCumulative]
	END
	
	------ Alter column type
	ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative ALTER COLUMN AuditID bigint


	----- recreate indexes
	CREATE NONCLUSTERED INDEX [NIDX_QOSHRSID] ON [eddsdbo].[QoS_VarscatOutputDetailCumulative]
	(
		[QoSHourID] ASC,
		[SearchArtifactID] ASC
	)
	INCLUDE ( 	[ServerName],
		[SummaryDayHour],
		[CaseArtifactID],
		[UserID],
		[ExecutionTime],
		[QoSAction],
		[IsLongRunning],
		[IsComplex],
		[AuditID],
		[SearchName],
		[QueryID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


		CREATE NONCLUSTERED INDEX [IX_HourAction] ON [eddsdbo].[QoS_VarscatOutputDetailCumulative]
		(
			[QoSHourID] ASC,
			[QoSAction] ASC
		)
		INCLUDE ( 	[SummaryDayHour],
			[CaseArtifactID],
			[SearchArtifactID],
			[SearchName],
			[AuditID],
			[UserID],
			[ExecutionTime],
			[IsLongRunning],
			[IsComplex],
			[ServerName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END