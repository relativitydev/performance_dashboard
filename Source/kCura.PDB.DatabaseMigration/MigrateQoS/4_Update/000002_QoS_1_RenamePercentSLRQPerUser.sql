--Upgrade Script QoS_1 change PercentSLRQPerUser to AvgPercentSQPerUser

--this needs to run against each EDDSQoS database
--EDDSDBO.QoS_UserXServerSummary
--BusiestWorkspace (change to INT) 

	IF EXISTS (SELECT 1 FROM EDDSQoS.INFORMATION_SCHEMA.COLUMNS
		WHERE TABLE_NAME = 'QoS_UserXServerSummary' AND COLUMN_NAME = 'PercentSLRQPerUser')
	BEGIN
		--add the new columns: BusiestUser and AvgSQScorePerUser
		EXEC('ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary add BusiestUser INT 
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary add AvgSQScorePerUser DECIMAL (18, 0)')
		
		--Migrate existing data from PercentSLRQPerUser to the new AvgSQScorePerUser
		--Create placeholder for temporary BusiestWorkspace data
		EXEC('UPDATE EDDSQoS.EDDSDBO.QoS_UserXServerSummary SET AvgSQScorePerUser = PercentSLRQPerUser
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary DROP COLUMN PercentSLRQPerUser

		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary add BusiestWorkspace1 INT ')
		
		--Migrate data from old BusiestWorkspace column, drop it, and migrate to a new column of type INT
		EXEC('UPDATE EDDSQoS.EDDSDBO.QoS_UserXServerSummary SET BusiestWorkspace1 = CAST(SUBSTRING(BusiestWorkspace, 5, 100) as int)
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary DROP COLUMN BusiestWorkspace --(DROP the varchar column)
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary add BusiestWorkspace INT 
		UPDATE EDDSQoS.EDDSDBO.QoS_UserXServerSummary SET BusiestWorkspace = BusiestWorkspace1
		ALTER TABLE EDDSQoS.EDDSDBO.QoS_UserXServerSummary DROP COLUMN BusiestWorkspace1')
	END

