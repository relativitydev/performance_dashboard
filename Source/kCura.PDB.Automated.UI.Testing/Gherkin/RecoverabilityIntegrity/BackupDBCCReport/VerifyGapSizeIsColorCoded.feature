Feature: VerifyGapSizeIsColorCoded
	
@BackupDbccReportPage
Scenario: Verify gap size is coded in red color on Backup/DBCC Report page.
	Given I upload Mock data that generates Gap size of 13 days with Gap Resolution Date N/A.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the Gap size of '13' days with Gap Resolution Date 'N/A' is coded in 'Red' color.

@BackupDbccReportPage
Scenario: Verify gap size is coded in yellow color on Backup/DBCC Report page.
	Given I upload Mock data that generates Gap size of 10 days with Gap Resolution Date.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the Gap size of '10' days with 'Gap Resolution Date' is coded in 'Yellow' color.

@BackupDbccReportPage
Scenario: Verify gap size is coded in green color on Backup/DBCC Report page.
	Given I upload Mock data that generates Gap size of 9 days.
	When I have navigated to the Backup/DBCC Report subpage.
	Then the user can see the Gap size of '9' days is coded in 'Green' color.