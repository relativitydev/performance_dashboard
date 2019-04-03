Feature: VerifyBackupsCoverageScore
    
@QualityofServicePage
Scenario: Verify Backups Coverage Score for RI Summary Report.
    Given I upload Mock data that generates a Backups Coverage Score.
    When I have navigated to the "Performance Dashboard" page.
    Then I can see the Backups Coverage Score is valid.