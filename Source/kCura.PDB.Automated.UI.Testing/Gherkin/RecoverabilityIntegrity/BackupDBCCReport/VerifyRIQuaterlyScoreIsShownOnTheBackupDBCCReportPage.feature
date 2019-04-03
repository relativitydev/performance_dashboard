Feature: VerifyRIQuaterlyScoreIsShownOnTheBackupDBCCReportPage
	
@GoldenFlow @BackupDbccReportPage
Scenario: Verify RI Quarterly score is shown on the Backup/DBCC Report page.
	When I have navigated to the Backup/DBCC Report subpage.
	Then I can see the Quarterly RI Score is shown on the Backup/DBCC Report page.