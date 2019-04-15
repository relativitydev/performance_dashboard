Feature: VerifyTheListIsSortable
	
@GoldenFlow @RecoverabilityIntegrityPage
Scenario: Verify the list on Recoverability/Integrity page is sortable for all columns.
	When I have navigated to the "Recoverability/Integrity" page.
	Then the user can see the list is sortable for all columns.