USE EDDSPerformance
GO

IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'QoS_CasesToAudit' AND TABLE_SCHEMA = N'EDDSDBO') 
BEGIN
	CREATE TABLE EDDSDBO.QoS_CasesToAudit
	( 
		RowID INT IDENTITY ( 1 , 1 ) ,
		ServerName nVARCHAR (150),
		ServerID INT, 
		CaseArtifactID INT,
		DatabaseName nvarchar(128),
		WorkspaceName varchar(50),
		AuditStartDate Datetime,
		Retry INT, 
		IsCompleted BIT,
		IsActive BIT,
		IsFailedThisRun BIT,
		RowHash binary(20)
	)
END
GO