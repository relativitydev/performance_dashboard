

SELECT CASE WHEN EXISTS(
	SELECT *
		FROM eddsdbo.ProcessControl with(nolock)
		WHERE ProcessControlID = @id
			AND LastProcessExecDateTime >= @timeThreshold
			AND [LastExecSucceeded] = 1
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) END