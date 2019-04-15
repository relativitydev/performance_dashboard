--Upgrade Script QoS_4
--Change datatype of EDDSQoS.eddsdbo.QoS_ConcurrencyItems.WorkspaceID from varchar to int
--Change name of EDDSQoS.eddsdbo.QoS_UserXServerSummary.PercentCLRQPerUser to AvgCQScorePerUser

USE EDDSQoS;
GO

	IF EXISTS (SELECT 1 FROM EDDSQoS.INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME = 'QoS_ConcurrencyItems' AND COLUMN_NAME = 'WorkspaceID' AND DATA_TYPE = 'varchar')
	BEGIN		
		--Create placeholder for temporary WorkspaceID data
		EXEC('ALTER TABLE EDDSQoS.EDDSDBO.QoS_ConcurrencyItems add WorkspaceID2 INT')
		
		--Migrate data from old WorkspaceID column and drop it
		EXEC('UPDATE EDDSQoS.EDDSDBO.QoS_ConcurrencyItems SET WorkspaceID2 = CAST(SUBSTRING(WorkspaceID, 5, 100) as int)
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_ConcurrencyItems DROP COLUMN WorkspaceID --(DROP the varchar column)')
		
		--Add the new column
		EXEC('ALTER TABLE EDDSQoS.EDDSDBO.QoS_ConcurrencyItems ADD WorkspaceID INT')
		
		--Migrate data to the new column and drop the temporary one
		EXEC('UPDATE EDDSQoS.EDDSDBO.QoS_ConcurrencyItems SET WorkspaceID = WorkspaceID2
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_ConcurrencyItems DROP COLUMN WorkspaceID2')
	END
	
	IF EXISTS (SELECT 1 FROM EDDSQoS.INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME = 'QoS_UserXServerSummary' AND COLUMN_NAME = 'PercentCLRQPerUser')
	BEGIN
		--add the new column: AvgCQScorePerUser
		EXEC('ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary add AvgCQScorePerUser DECIMAL (18, 0)')
		
		--Migrate existing data from PercentCLRQPerUser to the new AvgCQScorePerUser, then drop the old column
		EXEC('UPDATE EDDSQoS.EDDSDBO.QoS_UserXServerSummary SET AvgCQScorePerUser = PercentCLRQPerUser
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary DROP COLUMN PercentCLRQPerUser')
	END

