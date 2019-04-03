USE EDDSPerformance
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'SummaryDayHour') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		ADD SummaryDayHour DATETIME; 
	END

	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'CaseArtifactID') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		ADD CaseArtifactID INT;
	END
END

GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'SummaryDayHour') IS NOT NULL
	BEGIN
		UPDATE eddsdbo.QoS_VarscatOutputDetailCumulative
		SET SummaryDayHour = DATEADD(HH, DATEDIFF(HH, 0, Timestamp), 0);
	END
END

GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF (COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'CaseArtifactID') IS NOT NULL
		AND COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'DatabaseName') IS NOT NULL)
	BEGIN
		DECLARE @SQL varchar(max) = '
		UPDATE eddsdbo.QoS_VarscatOutputDetailCumulative
		SET CaseArtifactID = SUBSTRING(DatabaseName, 5, 10);
		
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		DROP COLUMN DatabaseName;'
		
		EXEC(@SQL);
	END
END