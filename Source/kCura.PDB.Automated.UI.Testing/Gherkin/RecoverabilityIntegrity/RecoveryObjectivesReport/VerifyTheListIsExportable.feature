Feature: VerifyTheListIsExportable
	
@RecoveryObjectivesReportPage
Scenario: Verify the list on Recovery Objectives Report page is exportable.
	When I have navigated to the Recovery Objectives Report subpage.
	Then the user can see the 'RecoveryObjectives' list is exportable on the Recovery Objectives Report page and the file exists at the 'Download path'.