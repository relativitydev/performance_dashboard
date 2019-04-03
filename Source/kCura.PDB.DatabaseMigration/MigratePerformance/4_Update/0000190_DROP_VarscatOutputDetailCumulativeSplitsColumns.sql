use EDDSPerformance

IF EXISTS (SELECT TOP 1 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_VarscatOutputDetailCumulative]') AND name = 'IX_SummaryDayHour_CaseArtifactID')
BEGIN
	DROP INDEX IX_SummaryDayHour_CaseArtifactID
	ON eddsdbo.QoS_VarscatOutputDetailCumulative
END

IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetailCumulative' ,'Split') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
    DROP COLUMN Split
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetailCumulative' ,'Bound') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
    DROP COLUMN Bound
END


IF COL_LENGTH ('eddsdbo.QoS_VarscatOutputDetailCumulative' ,'Finish') IS NOT NULL
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
    DROP COLUMN Finish
END