Feature: VerifyTheListIsExportable
	
@RecoverabilityIntegrityPage
Scenario: Verify the list on Recoverability/Integrity page is exportable.
	When I have navigated to the "Recoverability/Integrity" page.
	Then the user can see the 'RecoverabilityIntegrity' list is exportable on the Recoverability/Integrity page and the file exists at the 'Download path'.