Feature: VerifyMaxDataLossScore
	
@QualityofServicePage
Scenario: Verify Max Data Loss Score for RI Summary Report.
	Given I upload Mock data that generates a Max Data Loss Score.
	When I have navigated to the "Performance Dashboard" page.
	Then I can see the Max Data Loss Score is valid.