-- use [EDDSPerformance];
select cs.* from eddsdbo.CategoryScores as cs with(nolock)
where cs.CategoryID = @categoryId