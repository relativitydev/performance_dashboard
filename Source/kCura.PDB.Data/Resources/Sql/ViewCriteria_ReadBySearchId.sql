--DECLARE @searchArtifactId int = 1065485

SELECT DISTINCT
	vc.viewCriteriaID,
    vc.Value,
    case when vc.Operator = 'like' and f.fieldTypeName in ('Multiple Choice', 'Single Choice') -- Fix bogus "Like" operator
		then 'choiceSearch' else vc.Operator end as Operator,
	case when vf.ArtifactViewFieldID is null then 1 else 0 end as IsSubQuery
FROM eddsdbo.[View] v WITH(NOLOCK)
LEFT JOIN eddsdbo.ViewCriteria vc WITH(NOLOCK)
	ON v.ArtifactID = vc.viewID
LEFT JOIN eddsdbo.ExtendedField f WITH(NOLOCK)
	ON f.ArtifactViewFieldID = vc.ArtifactViewFieldID
LEFT JOIN eddsdbo.ViewField as vf WITH(NOLOCK)
	ON vf.ArtifactViewFieldID = vc.ArtifactViewFieldID
WHERE v.ArtifactTypeID = 10 
	AND vc.ViewCriteriaID IS NOT NULL
	AND v.ArtifactID = @searchArtifactId