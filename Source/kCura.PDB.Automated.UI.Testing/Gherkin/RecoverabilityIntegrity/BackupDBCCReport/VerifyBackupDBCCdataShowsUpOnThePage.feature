Feature: VerifyBackupDBCCdataShowsUpOnThePage

@BackupDbccReportPage
Scenario: Verify Backup/DBCC data shows up on the page.
	Given I upload Mock data that generates Backup/DBCC report data.
	When I have navigated to the Backup/DBCC Report subpage.
	Then I can see the Backup/DBCC report data shows up on the page.