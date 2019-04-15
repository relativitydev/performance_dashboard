Feature: VerifyTheListIsFiltarable
	
@RecoveryObjectivesReportPage
Scenario: Verify the list on Recovery Objectives Report page is filtarable for all columns.
	Given I upload Mock data that generates Recovery Objectives report data.
	When I have navigated to the Recovery Objectives Report subpage.
	Then the user can see the list is filtarable for all columns on the Recovery Objectives Report page.