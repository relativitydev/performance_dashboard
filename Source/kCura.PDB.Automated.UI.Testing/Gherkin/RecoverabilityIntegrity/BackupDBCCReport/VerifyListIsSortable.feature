Feature: VerifyListIsSortable
	
@GoldenFlow @BackupDbccReportPage
Scenario: Verify the list on Backup/DBCC Report page is sortable for all columns.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the list is sortable for all columns on the Backup/DBCC Report page.