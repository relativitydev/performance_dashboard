USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_ActiveHours' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_ActiveHours
	(
		ActiveHoursID INT IDENTITY ( 1 , 1 ),PRIMARY KEY (ActiveHoursID),
		AuditStartDate DATETIME
	)
END
GO