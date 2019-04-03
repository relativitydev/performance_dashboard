USE EDDSQoS;
GO

/*
QoS_UserExperienceSearchSummary (EDDSPerformance)
QoS_VarscatOutputDetailCumulative (EDDSPerformance)
QoS_VarscatOutputDetail (EDDSQoS)
VarscatOutputDetail (workspace DB)
QoS_SearchAuditRows (workspace DB)
QoS_SearchAuditParsed (workspace DB)
*/

IF exists(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'QoS_VarscatOutputDetail' AND COLUMN_NAME = 'AuditID' AND DATA_TYPE = 'INT')
BEGIN
    ALTER TABLE eddsdbo.QoS_VarscatOutputDetail ALTER COLUMN AuditID bigint
END

