USE EDDSPerformance
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'SearchName') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetailCumulative
		ADD SearchName nvarchar(max)
	END
END

GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetailCumulative' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetailCumulative', 'SearchName') IS NOT NULL
	BEGIN
		UPDATE VODC
		SET VODC.SearchName = VOC.SearchName
		FROM eddsdbo.QoS_VarscatOutputDetailCumulative VODC
		INNER JOIN eddsdbo.QoS_VarscatOutputCumulative VOC
		ON VODC.QoSHourID = VOC.QoSHourID AND VODC.SearchArtifactID = VOC.SearchArtifactID
		WHERE VODC.SummaryDayHour > DATEADD(dd, -90, getutcdate())
		AND VODC.QoSAction IN (281, 282)
	END
END