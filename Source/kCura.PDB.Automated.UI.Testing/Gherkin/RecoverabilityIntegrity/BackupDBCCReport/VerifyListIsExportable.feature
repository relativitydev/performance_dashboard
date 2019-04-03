Feature: VerifyListIsExportable
	
@BackupDbccReportPage
Scenario: Verify the list on Backup/DBCC Report page is exportable.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the 'BackupDBCC' list is exportable on the Backup/DBCC Report page and the file exists at the 'Download path'.