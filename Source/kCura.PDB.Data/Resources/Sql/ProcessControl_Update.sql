

UPDATE [eddsdbo].[ProcessControl]
   SET [ProcessControlID] = @ProcessControlID
      ,[ProcessTypeDesc] = @ProcessTypeDesc
      ,[LastProcessExecDateTime] = @LastProcessExecDateTime
      ,[Frequency] = @Frequency
      ,[LastExecSucceeded] = @LastExecSucceeded
      ,[LastErrorMessage] = @LastErrorMessage
WHERE ProcessControlID = @id