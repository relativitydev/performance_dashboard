Feature: NavigateToPDBScoringPages

@GoldenFlow @Navigation @UserExperiencePage @InfrastructurePerformancePage @Recoverability/IntegrityPage @UptimePage
Scenario Outline: Navigation to the scoring pages.
	When I navigate to the '<Target Page>' page.
	Then the '<Scoring Page>' Page with the score loads.

Examples:
	| Target Page                | Scoring Page                      |
	| User Experience            | User Experience Report            |
	| Infrastructure Performance | Infrastructure Performance Report |
	| Recoverability/Integrity   | Recoverability/Integrity Report   |
	| Uptime                     | Uptime Report                     | 