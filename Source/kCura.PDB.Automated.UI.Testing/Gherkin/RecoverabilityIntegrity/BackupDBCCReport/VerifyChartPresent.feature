Feature: VerifyChartPresent

@GoldenFlow @BackupDbccReportPage
Scenario: Verify chart is present on the Backup/DBCC Report page.
	When I have navigated to the Backup/DBCC Report subpage.
	Then I can see the Chart is present on the Backup/DBCC Report page.