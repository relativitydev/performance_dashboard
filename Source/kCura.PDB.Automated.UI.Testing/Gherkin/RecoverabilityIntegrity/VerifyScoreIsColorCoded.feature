Feature: VerifyScoreIsColorCoded

@RecoverabilityIntegrityPage
Scenario: Verify score is coded in red color on the Recoverability/Integrity page.
	Given I upload Mock data that generates RI Score of 0.
	When I have navigated to the "Recoverability/Integrity" page.
	Then the user can see the score of '0' is coded in 'Red' color.

@RecoverabilityIntegrityPage
Scenario: Verify score is coded in yellow color on the Recoverability/Integrity page.
	Given I upload Mock data that generates RI Score of 89.
	When I have navigated to the "Recoverability/Integrity" page.
	Then the user can see the score of '89' is coded in 'Yellow' color.

@RecoverabilityIntegrityPage
Scenario: Verify score is coded in green color on the Recoverability/Integrity page.
	Given I upload Mock data that generates RI Score of 100.
	When I have navigated to the "Recoverability/Integrity" page.
	Then the user can see the score of '100' is coded in 'Green' color.