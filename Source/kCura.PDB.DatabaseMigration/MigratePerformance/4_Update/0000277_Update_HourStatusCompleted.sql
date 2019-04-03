USE [EDDSPerformance];

update eddsdbo.[hours]
set [Status] = 3
where HourTimeStamp in (select summarydayhour from eddsdbo.QoS_Ratings)