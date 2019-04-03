USE [{{resourcedbname}}]
GO
IF EXISTS (select 1 from sysobjects where [name] = 'QoS_VirtualLogFileCounter' and type = 'P')  
BEGIN
	DROP PROCEDURE eddsdbo.QoS_VirtualLogFileCounter
END
GO
CREATE PROCEDURE eddsdbo.QoS_VirtualLogFileCounter
	@DatabaseName nvarchar(150)
WITH EXEC AS SELF
AS
BEGIN

DECLARE @SQL nvarchar(max) = N'
	USE ' + QUOTENAME(@DatabaseName) + '

	DBCC LOGINFO

	INSERT INTO EDDSQoS.eddsdbo.VirtualLogFileDW
		(DatabaseName, VirtualLogFiles)
	VALUES
		(@DatabaseName, @@ROWCOUNT)
	',
	@parmDefinition nvarchar(max) = N'@DatabaseName NVARCHAR(150)';

EXEC sp_executesql @SQL, @parmDefinition, @DatabaseName;

END