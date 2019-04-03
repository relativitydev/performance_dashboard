Feature: VerifyTheListIsPagable

@RecoverabilityIntegrityPage
Scenario: Verify the list on Recoverability/Integrity page is pagable.
	Given I upload Mock data that generates RI Score.
	When I have navigated to the "Recoverability/Integrity" page.
	Then the user can see the list is pagable.