--Upgrade Script QoS_5
--Rename EDDSPerformance.eddsdbo.QoS_UserXInstanceSummary.PercentCLRQPerUser to AvgCQScorePerUser

USE EDDSPerformance;
GO

IF EXISTS (SELECT 1 FROM EDDSPerformance.INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME = 'QoS_UserXInstanceSummary' AND COLUMN_NAME = 'PercentCLRQPerUser')
	BEGIN
		--add the new column: AvgCQScorePerUser
		EXEC('ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary ADD AvgCQScorePerUser DECIMAL (18, 0)')
		
		--Migrate existing data from PercentCLRQPerUser to the new AvgCQScorePerUser
		EXEC('UPDATE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary SET AvgCQScorePerUser = PercentCLRQPerUser
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary DROP COLUMN PercentCLRQPerUser')
	END