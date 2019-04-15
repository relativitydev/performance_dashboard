Feature: VerifyTimeToRecover

@QualityofServicePage
Scenario: Verify Time To Recover for RI Summary Report. 
	Given I upload Mock data that generates a Time to Recover Time.
	When I have navigated to the "Performance Dashboard" page.
	Then I can see the Time To Recover Time is valid.