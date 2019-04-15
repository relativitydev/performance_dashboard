-- EDDSPerformance

SELECT s.*
FROM eddsdbo.[MockServer] ms
INNER JOIN eddsdbo.[Server] s on ms.ServerId = s.ServerId