Feature: RetryErroredEvents

@retryButton @BackfillPage
Scenario: Admin retries errored events on the Backfill page.
	Given that I generate an Errored Event.
	And that I am on the Backfill Console page.
	When I press the Retry Errored Events button.
	Then there are no errored events remaining.

@GoldenFlow @retryButton @BackfillPage
Scenario: Admin does not see Retry Errored Events button on the Backfill page when no errors exist.
	Given that I am on the Backfill Console page.
	When there are no errored events remaining.
	Then the Retry Errored Events button is not present.