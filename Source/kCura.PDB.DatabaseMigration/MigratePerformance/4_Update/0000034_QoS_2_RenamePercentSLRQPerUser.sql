--This needs to run against the EDDSPerformance only

	IF EXISTS (SELECT 1 FROM EDDSPerformance.INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME = 'QoS_UserXInstanceSummary' AND COLUMN_NAME = 'PercentSLRQPerUser')
	BEGIN
		--add the new columns: BusiestUser and AvgSQScorePerUser
		EXEC('ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary add BusiestUser INT 
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary add AvgSQScorePerUser DECIMAL (18, 0)')
		
		--Migrate existing data from PercentSLRQPerUser to the new AvgSQScorePerUser
		--Create placeholder for temporary BusiestWorkspace data
		EXEC('UPDATE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary SET AvgSQScorePerUser = PercentSLRQPerUser
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary DROP COLUMN PercentSLRQPerUser

		--change Busiest workspace column type to INT
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary add BusiestWorkspace1 INT ')
		
		--Migrate data from old BusiestWorkspace column, drop it, and migrate to a new column of type INT
		EXEC('UPDATE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary SET BusiestWorkspace1 = CAST(SUBSTRING(BusiestWorkspace, 5, 100) as int)
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary DROP COLUMN BusiestWorkspace --(DROP the varchar column)
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary add BusiestWorkspace INT 
		UPDATE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary SET BusiestWorkspace = BusiestWorkspace1
		ALTER TABLE EDDSPerformance.EDDSDBO.QoS_UserXInstanceSummary DROP COLUMN BusiestWorkspace1')
	END

