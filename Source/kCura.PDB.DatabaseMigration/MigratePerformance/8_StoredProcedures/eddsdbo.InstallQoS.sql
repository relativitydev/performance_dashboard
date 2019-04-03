 IF EXISTS ( SELECT 1 FROM sysobjects
            WHERE [name] = 'InstallQoS'
                    AND type = 'P' ) 
BEGIN
	DROP PROCEDURE eddsdbo.InstallQoS
END
GO

CREATE PROCEDURE EDDSDBO.InstallQoS
 @Install VARCHAR (7)
 AS
 BEGIN 

 DECLARE @DropSQL NVARCHAR(MAX) = N'';
 IF @install='new' --drop everything except GlassRun History
 BEGIN
 Update [eddsdbo].[QoS_GlassRunHistory] SET isActive = 0
SELECT @DropSQL += CASE WHEN [type] ='U' THEN 'DROP TABLE ' 
							WHEN [type] = 'P' THEN 'DROP PROCEDURE '
							WHEN [type] = 'FN' THEN 'DROP FUNCTION ' END
+ QUOTENAME(OBJECT_SCHEMA_NAME([object_id]))
+ '.' + QUOTENAME(name) + ';
'
FROM sys.objects
WHERE (name LIKE 'QoS%' or name = 'concurrencyServer') and name not in ('QoS_GlassRunHistory') and [type] not in ('FN','P')
PRINT @dropSQL
EXEC sp_executesql @DropSQL;
END
END