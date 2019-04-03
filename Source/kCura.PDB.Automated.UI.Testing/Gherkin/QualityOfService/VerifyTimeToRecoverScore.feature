Feature: VerifyTimeToRecoverScore
	
@QualityofServicePage
Scenario: Verify Time to Recover Score for RI Summary Report.
	Given I upload Mock data that generates a Time to Recover Score.
	When I have navigated to the "Performance Dashboard" page.
	Then I can see the Time to Recover Score is valid.