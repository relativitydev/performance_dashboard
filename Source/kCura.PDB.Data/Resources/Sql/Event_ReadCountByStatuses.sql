

select count(id) from [eddsdbo].[Events] with(nolock)
where StatusId in @status
