/*

Process for 7.5 installation:

1. Delete any existing Performance Dashboard agents.
2. If a QoS table purge is needed, purge the tables manually.
3. Import the 7.5 RAP file.
4. If EDDSPerformance does not exist, create the PDB agent and allow it to create EDDSPerformance. It will stop after this due to the assembly file version mismatch. Stop the agent once EDDSPerformance exists.
5. Run this script to fix the assembly file version mismatch and stage EDDSQoS and workspace-level deployment.
6. If a backfill is desired, start executing QoS_Backfill.
7. Create or start the agent to allow EDDSQoS and workspace-level deployment to run. During this time, check for any errors in RHScriptsRun in EDDSPerformance. If EDDSQoS deployment begins and there are no errors in RHScriptsRun, you may proceed.
8. Navigate to PDB's custom pages and install the backup/DBCC procedures.
9. If a backfill is running, you should begin monitoring it at this time. Looking Glass should be called as soon as deployment tasks have completed and the latest backup/DBCC procedures have been installed. If Looking Glass tables do not appear in EDDSPerformance, check the LastProcessExecDateTime for the workspace and server deployment tasks in the ProcessControl table and compare the configuration table values for AdminScriptsVersion and AdminScriptsLatestVersion.

*/

USE EDDSPerformance
GO

DECLARE @assemblyFileVersion varchar(max) = '2014.12.2.22874';
	
IF NOT EXISTS (SELECT TOP 1 Name FROM EDDSPerformance.eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'AssemblyFileVersion')
BEGIN
	INSERT INTO EDDSPerformance.eddsdbo.Configuration
		(Section, Name, Value, MachineName, [Description])
	VALUES
		('kCura.PDB', 'AssemblyFileVersion', @assemblyFileVersion, '', '')
END
ELSE
BEGIN
	UPDATE EDDSPerformance.eddsdbo.Configuration
	SET Value = @assemblyFileVersion
	WHERE Section = 'kCura.PDB' AND Name = 'AssemblyFileVersion'
END

UPDATE EDDSPerformance.eddsdbo.ProcessControl
SET LastProcessExecDateTime = '2014-09-01 00:00:00.000'
WHERE ProcessControlID IN (5, 6)