DECLARE @testSearchText table(searchText nvarchar(MAX), artifactId int);
DECLARE @testParsedText table(parsedText nvarchar(MAX));

--INSERT @testSearchText(searchText, artifactId)
SELECT TOP 100
	v.SearchText,
	vc.viewCriteriaID,
	a.[ArtifactID],
	au.FullName,
	a.CreatedOn,
	vc.Value,
	f.ArtifactViewFieldID,
	f.DisplayName,
	vc.Operator,
	v.QueryHint,
	a.TextIdentifier,
	LEN(v.SearchText) SearchTextLength,
	--v.SearchText,
	f.FieldTypeID AS SearchFieldTypeID
    --@testSearchText = v.SearchText
FROM eddsdbo.Artifact a WITH(NOLOCK)
--INNER JOIN EDDSDBO.AuditRecord ar WITH(NOLOCK)
--	ON ar.ArtifactID = a.ArtifactID
INNER JOIN eddsdbo.[View] v WITH(NOLOCK)
	ON a.ArtifactID = v.ArtifactID
LEFT JOIN eddsdbo.ViewCriteria vc WITH(NOLOCK)
	ON v.ArtifactID = vc.viewID
INNER JOIN eddsdbo.AuditUser au WITH(NOLOCK)
	ON a.CreatedBy = au.UserID
--LEFT JOIN eddsdbo.ArtifactViewField avf WITH(NOLOCK)
--	ON vc.ArtifactViewFieldID = avf.ArtifactViewFieldID
LEFT JOIN eddsdbo.field f WITH(NOLOCK)
	ON vc.ArtifactViewFieldID = f.ArtifactViewFieldID
WHERE v.ArtifactTypeID IN ( 10 )  --history artifacttypeid = 1000003
AND SearchText <> ''
ORDER BY a.ArtifactID desc
--GROUP BY a.ArtifactID



SELECT * FROM @testSearchText
INSERT @testParsedText(parsedText)
	SELECT
		CASE WHEN searchText LIKE '%SQLServer2005SearchProvider%'
			THEN ISNULL(CAST(searchText AS XML).value('declare namespace CRUD="kCura.EDDS.SQLServer2005SearchProvider";(/InputData/CRUD:SearchText)[1]', 'nvarchar(max)'), '')
		WHEN searchText LIKE '%DTSearchSearchProvider%'
			THEN ISNULL(CAST(searchText AS XML).value('declare namespace CRUD="kCura.EDDS.DTSearchSearchProvider";(/InputData/CRUD:SearchText)[1]', 'nvarchar(max)'), '')
		WHEN searchText LIKE '%ContentAnalystSearchProvider%'
			THEN ISNULL(CAST(searchText AS XML).value('declare namespace CRUD="kCura.EDDS.ContentAnalystSearchProvider";(/InputData/CRUD:KeywordsText)[1]', 'nvarchar(max)'), '')
		WHEN searchText LIKE '%ContentAnalystSearchProvider%'
			THEN ISNULL(CAST(searchText AS XML).value('declare namespace CRUD="kCura.EDDS.ContentAnalystSearchProvider";(/InputData/CRUD:ConceptsText)[1]', 'nvarchar(max)'), '')
		END
						
FROM @testSearchText
SELECT * FROM @testParsedText