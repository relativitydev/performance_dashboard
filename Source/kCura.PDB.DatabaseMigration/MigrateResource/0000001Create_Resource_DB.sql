/*==========================INDICATE SCRIPT RESULTS =================================
==	RoundhousE expects an output indicating the results of running this script.	   ==
==																				   ==
==	1: Database was created successfully (this run)								   ==
==	0: Database was not created successfully or already existed					   ==
===================================================================================*/
USE [{{resourcedbname}}]
IF NOT EXISTS (SELECT * FROM [{{resourcedbname}}].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RHVersion') BEGIN
	SELECT 1
END ELSE BEGIN
	SELECT 0
END