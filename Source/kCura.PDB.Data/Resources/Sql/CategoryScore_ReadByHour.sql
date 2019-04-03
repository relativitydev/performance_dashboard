

select cs.* from eddsdbo.CategoryScores as cs with(nolock)
inner join eddsdbo.Categories as c with(nolock) on c.ID = cs.CategoryID
where c.HourID = @hourId
