Feature: VerifyChartPresent

@GoldenFlow @RecoverabilityIntegrityPage
Scenario: Verify chart is present on the Recoverability/Integrity page.
	When I have navigated to the "Recoverability/Integrity" page.
	Then I can see the Chart is present on the page.