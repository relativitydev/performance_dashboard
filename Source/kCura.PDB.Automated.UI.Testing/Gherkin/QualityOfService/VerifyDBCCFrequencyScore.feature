Feature: VerifyDBCCFrequencyScore
	
@QualityofServicePage
Scenario: Verify DBCC Frequency Score for RI Summary Report.
	Given I upload Mock data that generates a DBCC Frequency Score.
	When I have navigated to the "Performance Dashboard" page.
	Then I can see the DBCC Frequency Score is valid.