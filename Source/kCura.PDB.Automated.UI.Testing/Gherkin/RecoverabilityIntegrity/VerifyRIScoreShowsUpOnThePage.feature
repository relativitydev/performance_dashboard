Feature: VerifyRIScoreShowsUpOnThePage

@RecoverabilityIntegrityPage
Scenario: Verify RI score shows up on the page.
	Given I upload Mock data that generates RI Score.
	When I have navigated to the "Recoverability/Integrity" page.
	Then I can see the RI Score shows up on the page.