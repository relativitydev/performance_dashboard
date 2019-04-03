USE EDDSPerformance --Change database if you want to put a trigger on something in EDDSQoS or a workspace database
GO

IF EXISTS (SELECT * FROM sys.sysobjects WHERE Name = 'HappyTrigger' AND Type = 'TR')
	DROP TRIGGER eddsdbo.HappyTrigger;
GO
CREATE TRIGGER HappyTrigger
ON eddsdbo.QoS_Ratings --Change table name here
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM EDDSQoS.sys.tables WHERE name = 'TriggerLog')
	CREATE TABLE EDDSQoS.eddsdbo.TriggerLog (
		TLID [int] IDENTITY(1,1) NOT NULL,
		PRIMARY KEY (TLID),
		LogTime datetime NOT NULL
	);
	
	INSERT INTO EDDSQoS.eddsdbo.TriggerLog (LogTime) VALUES (getutcdate());
END
GO