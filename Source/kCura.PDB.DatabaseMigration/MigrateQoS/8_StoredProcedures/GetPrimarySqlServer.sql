USE EDDSQoS
GO

IF EXISTS (select 1 from sysobjects where [name] = 'GetPrimarySqlServer' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.GetPrimarySqlServer
END

/* Deprecated 2/14/2018 */