

-- we want to read the hour if ALL of the category scores are scored so we are checking if there are any category scores that are NOT scored and then only taking the hour if there are none of those

select distinct *
	from eddsdbo.Hours with(nolock)
	where ID = @hourId
	and ID not in (
		select distinct h.ID
		from eddsdbo.Hours as h with(nolock)
		inner join eddsdbo.Categories as c on c.HourID = h.ID
		inner join eddsdbo.CategoryScores as cs on c.ID = cs.CategoryID
		where h.ID = @hourId and cs.Score is null and Status != 4
	)
	and Status != 4