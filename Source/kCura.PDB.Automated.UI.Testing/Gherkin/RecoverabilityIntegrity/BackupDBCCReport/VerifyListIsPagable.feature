Feature: VerifyListIsPagable

@BackupDbccReportPage
Scenario: Verify the list on Backup/DBCC Report page is pagable.
	Given I upload Mock data that generates Backup/DBCC report data.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the list is pagable on the Backup/DBCC Report page.