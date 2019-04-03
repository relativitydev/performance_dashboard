Feature: VerifyRIScoreSummaryReportDisplayed

@QualityofServicePage @QuaterlyScoreSummaryReport 
Scenario: Verify Quaterly RI Score Summary Report displayed on the page.
	Given I upload Mock data that generates Quaterly RI score.
	When I have navigated to the "Performance Dashboard" page.
	Then I see Quaterly RI Score exist on the page.

@QualityofServicePage @WeeklyScoreSummaryReport 
Scenario: Verify Weekly RI Score Summary Report displayed on the page.
	Given I upload Mock data that generates Weekly RI score.
	When I have navigated to the "Performance Dashboard" page.
	Then I see Weekly RI Score exist on the page.