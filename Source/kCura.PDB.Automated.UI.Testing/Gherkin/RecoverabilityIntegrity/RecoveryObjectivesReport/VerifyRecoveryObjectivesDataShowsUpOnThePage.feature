Feature: VerifyRecoveryObjectivedDataShowsUpOnThePage
	
@RecoveryObjectivesReportPage
Scenario: Verify Recovery Objectives data shows up on the page
	Given I upload Mock data that generates Recovery Objectived report data.
	When I have navigated to the Recovery Objectives Report subpage.
	Then I can see the Recovery Objectives report data shows up on the page.