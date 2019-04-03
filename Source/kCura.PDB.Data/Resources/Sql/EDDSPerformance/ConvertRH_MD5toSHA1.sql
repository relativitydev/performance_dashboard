UPDATE rsr
SET rsr.text_hash = msr.text_hash
FROM [eddsdbo].[RHScriptsRun] rsr
INNER JOIN @modified msr
ON rsr.id = msr.id