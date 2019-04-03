use edds

delete from eddsdbo.[GroupTab]
where TabArtifactID in
	(SELECT top(1) t.ArtifactID
	FROM eddsdbo.[Tab] AS t
	inner join eddsdbo.ArtifactAncestry as aa on aa.ArtifactID = t.ArtifactID 
	inner join eddsdbo.Tab as t2 on t2.ArtifactID = aa.AncestorArtifactID
	WHERE t2.Name = 'Performance Dashboard' and t.Name = 'Application Performance')


delete from eddsdbo.[Tab]
where ArtifactID in
	(SELECT top(1) t.ArtifactID
	FROM eddsdbo.[Tab] AS t
	inner join eddsdbo.ArtifactAncestry as aa on aa.ArtifactID = t.ArtifactID 
	inner join eddsdbo.Tab as t2 on t2.ArtifactID = aa.AncestorArtifactID
	WHERE t2.Name = 'Performance Dashboard' and t.Name = 'Application Performance')