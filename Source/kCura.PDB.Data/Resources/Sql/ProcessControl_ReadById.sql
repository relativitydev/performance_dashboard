

SELECT [ProcessControlID]
      ,[ProcessTypeDesc]
      ,[LastProcessExecDateTime]
      ,[Frequency]
      ,[LastExecSucceeded]
      ,[LastErrorMessage]
FROM eddsdbo.ProcessControl with(nolock)
WHERE ProcessControlID = @id