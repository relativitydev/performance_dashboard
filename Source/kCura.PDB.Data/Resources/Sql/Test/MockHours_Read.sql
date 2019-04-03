-- EDDSPerformance

SELECT h.* 
FROM eddsdbo.[Hours] h
INNER JOIN eddsdbo.[MockHours] mh on h.Id = mh.HourId