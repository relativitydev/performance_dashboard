-- EDDSPerformance


/* 
declare @scopeFilter varchar(255)
declare @nameFilter varchar(255)
declare @descriptionFilter varchar(255)
declare @statusFilter varchar(255)
declare @recommendationFilter varchar(255)
declare @valueFilter varchar(255)
declare @sectionFilter varchar(255)
*/


select * from eddsdbo.EnvironmentCheckRecommendations
where
	(@scopeFilter is null or [Scope] like '%' + @scopeFilter + '%') 
and	(@nameFilter is null or [Name] like '%' + @nameFilter + '%')  
and	(@descriptionFilter is null or [Description] like '%' + @descriptionFilter + '%')  
and	(@statusFilter is null or [Status] like '%' + @statusFilter + '%')  
and	(@recommendationFilter is null or [Recommendation] like '%' + @recommendationFilter + '%') 
and	(@valueFilter is null or [Value] like '%' + @valueFilter + '%')
and (@sectionFilter is null or [Section] like '%' + @sectionFilter + '%')
