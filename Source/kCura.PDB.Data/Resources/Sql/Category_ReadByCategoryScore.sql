

select top(1) c.*
from eddsdbo.Categories c with(nolock)
inner join eddsdbo.CategoryScores cs with(nolock) on c.ID = cs.CategoryID
where cs.ID = @categoryScoreId
