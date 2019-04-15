Feature: RPO (Recovery Point Objective)
	In order to verify RPO scoring
	As a SQL Database Admin
	I want to be shown the execution results of RPO analysis

@specflowIntegration
Scenario: Calculate an hour RPO score of 100
	Given I import a mock data set that should return RPO score of 100
	And I clean BackupDbcc report data for the given hours
	When I execute the BackupDBCCService on the mock data
	Then the RPO score for the hour should be 100

@specflowIntegration
Scenario: Calculate in the new system an hour RPO score of 100
	Given I import a mock data set that should return RPO score of 100
	And I clean BackupDbcc report data for the given hours
	When the event system scores the Recoverability/Integrity category scored for the given hours
	Then the RPO category score for the hour should be 100
