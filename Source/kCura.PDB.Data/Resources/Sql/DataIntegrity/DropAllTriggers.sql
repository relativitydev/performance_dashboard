Declare @trgName nvarchar(500) 
Declare cur Cursor For Select [name] From sys.objects where type = 'tr' 
Open cur 
Fetch Next From cur Into @trgName 
While @@fetch_status = 0 
Begin 
 Exec('drop trigger ' + @trgName) 
 Fetch Next From cur Into @trgName 
End
Close cur 
Deallocate cur 