Feature: VerifyRIQuarterlyScoreIsShownOnThePage

@GoldenFlow @RecoverabilityIntegrityPage
Scenario: Verify RI Quarterly score is shown on the page.
	When I have navigated to the "Recoverability/Integrity" page.
	Then I can see the Quarterly RI Score is shown on the page.