

UPDATE [eddsdbo].[CategoryScores]
   SET [CategoryID] = @categoryID,
       [ServerID] = @serverID,
       [Score] = @score
 WHERE ID = @id