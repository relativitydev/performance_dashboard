USE [EDDSPerformance];

-- Change the ServerName column to nvarchar(255) since it was previously nvarchar(50)
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME = N'QoS_GapSummary' 
		AND TABLE_SCHEMA = N'EDDSDBO'
		AND COLUMN_NAME = N'ServerName'
		AND CHARACTER_MAXIMUM_LENGTH < 255) 
BEGIN
	-- Resize to ResourceServer.Name column's nvarchar(255)
	ALTER TABLE eddsdbo.QoS_GapSummary
	ALTER COLUMN ServerName nvarchar(255) NULL;
END