
DECLARE @Name VarChar(100),@Type VarChar(20), @Schema VarChar(20)
            SELECT @Name = 'EDDSWorkspace', @Type = 'VIEW', @Schema = 'eddsdbo'

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(@Schema + '.' +  @Name))
BEGIN
  DECLARE @SQL varchar(1000)
  SET @SQL = 'CREATE ' + @Type + ' ' + @Schema + '.' + @Name + ' AS SELECT * FROM sys.objects'
  EXECUTE(@SQL)
END 
PRINT 'Updating ' + @Type + ' ' + @Schema + '.' + @Name
GO

ALTER VIEW  [eddsdbo].[EDDSWorkspace]
AS
SELECT  
	[Case].ArtifactID		AS CaseArtifactID
	, [Case].[Name]			AS WorkspaceName
	, (
		CASE 
			WHEN CHARINDEX('\', [ResourceServer].Name) > 0 
				THEN SUBSTRING( [ResourceServer].Name, CHARINDEX('\', [ResourceServer].Name) + 1, LEN( [ResourceServer].Name ) )
			ELSE [ResourceServer].Name
		END		
	  ) AS [DatabaseLocation]
FROM [EDDS].[eddsdbo].[Case] AS [Case] WITH (NOLOCK)
	INNER JOIN [EDDS].[eddsdbo].[ResourceServer] [ResourceServer] WITH (NOLOCK)
		ON [ResourceServer].artifactId = [Case].ServerId
		WHERE [Case].ArtifactID > 0

