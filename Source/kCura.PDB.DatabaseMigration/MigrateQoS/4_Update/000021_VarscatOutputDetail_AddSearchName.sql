USE EDDSQoS
GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetail' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetail', 'SearchName') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.QoS_VarscatOutputDetail
		ADD SearchName nvarchar(max)
	END
END

GO

IF EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QoS_VarscatOutputDetail' AND TABLE_SCHEMA = 'EDDSDBO') 
BEGIN
	IF COL_LENGTH('eddsdbo.QoS_VarscatOutputDetail', 'SearchName') IS NOT NULL
	BEGIN
		UPDATE VOD
		SET VOD.SearchName = VO.SearchName
		FROM eddsdbo.QoS_VarscatOutputDetail VOD
		INNER JOIN eddsdbo.QoS_VarscatOutput VO
		ON VOD.QoSHourID = VO.QoSHourID AND VOD.SearchArtifactID = VO.SearchArtifactID
		WHERE VOD.QoSAction IN (281, 282)
	END
END