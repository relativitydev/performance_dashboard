Feature: VerifyBackupsFrequencyScore
	
@QualityofServicePage
Scenario: Verify Backups Frequency Score for RI Summary Report.
	Given I upload Mock data that generates a Backups Frequency Score.
	When I have navigated to the "Performance Dashboard" page.
	Then I can see the Backups Frequency Score is valid.