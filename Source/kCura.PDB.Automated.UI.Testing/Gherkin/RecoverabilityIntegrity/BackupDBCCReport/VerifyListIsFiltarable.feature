Feature: VerifyListIsFiltarable
	
@BackupDbccReportPage
Scenario: Verify the list on Backup/DBCC Report page is filtarable for all columns.
	Given I upload Mock data that generates Backup/DBCC report data.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the list is filtarable for all columns on the Backup/DBCC Report page.