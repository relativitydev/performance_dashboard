/*
	PDB QA Scripts:
	Enable Automatic Looking Glass Calls

	Author: Joseph Low
	Date: 7/14/2014

	Comments: Running the sample data generator script will disable automatic Looking Glass calls to prevent it from interfering with the test data.
		You should run this script to reenable automatic procedure calls after you're done.
*/

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET Frequency = 60
WHERE ProcessControlID = 7