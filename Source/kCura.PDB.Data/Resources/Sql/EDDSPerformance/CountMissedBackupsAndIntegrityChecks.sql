USE EDDSPerformance;

DECLARE
	@missedBackups INT = 0,
	@missedDbcc INT = 0,
	@failedServers INT = 0,
	@failedDatabases INT = 0,
	@maxDataLossMinutes INT = 0,
	@ttrHours DECIMAL(6,2) = 0,
	@backupFrequency DECIMAL (5,2) = 100,
	@backupCoverage DECIMAL (5,2) = 100,
	@dbccFrequency DECIMAL (5,2) = 100,
	@dbccCoverage DECIMAL (5,2) = 100,
	@rpoScore DECIMAL (5,2) = 100,
	@rtoScore DECIMAL (5,2) = 100;

SELECT @missedBackups = ISNULL((
	SELECT COUNT(DISTINCT DBName)
	FROM eddsdbo.QoS_BackSummary WITH(NOLOCK)
	WHERE GapResolvedDate IS NULL
), 0);

SELECT @missedDbcc = ISNULL((
	SELECT COUNT(DISTINCT DBName)
	FROM eddsdbo.QoS_DBCCSummary WITH(NOLOCK)
	WHERE GapResolvedDate IS NULL
), 0);

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_tempServers' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	SET @failedServers = ISNULL((SELECT COUNT(*) FROM eddsdbo.QoS_tempServers WITH(NOLOCK) WHERE Failed = 1), 0);
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_FailedDatabases' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	SET @failedDatabases = ISNULL((SELECT COUNT(*) FROM eddsdbo.QoS_FailedDatabases WITH(NOLOCK)), 0);
END

SELECT TOP 1
	@backupCoverage = ISNULL(BackupCoverageScore, 100),
	@backupFrequency = ISNULL(BackupFrequencyScore, 100),
	@dbccCoverage = ISNULL(DbccCoverageScore, 100),
	@dbccFrequency = ISNULL(DbccFrequencyScore, 100),
	@rpoScore = ISNULL(RPOScore, 100),
	@rtoScore = ISNULL(RTOScore, 100),
	@maxDataLossMinutes = ISNULL(PotentialDataLossMinutes, 0),
	@ttrHours = ISNULL(EstimatedTimeToRecoverHours, 0)
FROM eddsdbo.QoS_RecoverabilityIntegritySummary WITH(NOLOCK)
ORDER BY SummaryDayHour DESC

SELECT
	@missedBackups MissedBackups,
	@missedDbcc MissedDbcc,
	@failedServers FailedServers,
	@failedDatabases FailedDatabases,
	@backupFrequency BackupFrequencyScore,
	@backupCoverage BackupCoverageScore,
	@dbccFrequency DbccFrequencyScore,
	@dbccCoverage DbccCoverageScore,
	@maxDataLossMinutes MaxDataLossMinutes,
	@ttrHours TimeToRecoverHours,
	@rpoScore RPOScore,
	@rtoScore RTOScore;