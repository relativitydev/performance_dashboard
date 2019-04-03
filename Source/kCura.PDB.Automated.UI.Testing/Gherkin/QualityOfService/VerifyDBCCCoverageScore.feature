Feature: VerifyDBCCCoverageScore
    
@QualityofServicePage
Scenario: Verify DBCC Coverage Score for RI Summary Report.
    Given I upload Mock data that generates a DBCC Coverage Score.
	When I have navigated to the "Performance Dashboard" page.
    Then I can see the DBCC Coverage Score is valid.