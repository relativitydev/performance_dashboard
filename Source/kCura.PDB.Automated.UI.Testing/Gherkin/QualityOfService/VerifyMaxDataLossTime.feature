Feature: VerifyMaxDataLossTime

@QualityofServicePage
Scenario: Verify Max Data Loss Time for RI Summary Report.
	Given I upload Mock data that generates a Max Data Loss Time.
	When I have navigated to the "Performance Dashboard" page.
	Then I can see the Max Data Loss Time is valid. 