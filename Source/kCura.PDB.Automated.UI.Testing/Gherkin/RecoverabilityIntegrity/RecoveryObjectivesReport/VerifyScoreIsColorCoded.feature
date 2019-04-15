Feature: VerifyScoreIsColorCoded

@RecoveryObjectivesReportPage
Scenario: Verify score is coded in red color on the Recovery Objectives Report page.
	Given I upload Mock data that generates RTO Score of 79.
	When I have navigated to the Recovery Objectives Report subpage.
	Then the user can see the score of '79' is coded in 'Red' color on the Recovery Objectives Report page.  

@RecoveryObjectivesReportPage
Scenario: Verify score is coded in yellow color on the Recovery Objectives Report page.
	Given I upload Mock data that generates RTO Score of 80.
	When I have navigated to the Recovery Objectives Report subpage.
	Then the user can see the score of '80' is coded in 'Yellow' color on the Recovery Objectives Report page.  

@RecoveryObjectivesReportPage
Scenario: Verify score is coded in green color on the Recovery Objectives Report page.
	Given I upload Mock data that generates RTO Score of 90.
	When I have navigated to the Recovery Objectives Report subpage.
	Then the user can see the score of '90' is coded in 'Green' color on the Recovery Objectives Report page.  