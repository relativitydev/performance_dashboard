DECLARE @friendlyName varchar(max) = 'DELETED: EDDS' + CAST(@CaseArtifactID as varchar) + '';

SELECT TOP 1
	@friendlyName = Name
FROM EDDS.eddsdbo.[Case] WITH(NOLOCK)
WHERE ArtifactID = @CaseArtifactID;

SELECT @friendlyName;