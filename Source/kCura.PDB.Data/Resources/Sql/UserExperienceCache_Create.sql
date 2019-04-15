

INSERT INTO eddsdbo.[UserExperience] 
	(HourId
	,ServerId
	,ActiveUsers
	,HasPoisonWaits
	,ArrivalRate
	,Concurrency)
VALUES 
	(@hourId
	,@serverId
	,@activeUsers
	,@hasPoisonWaits
	,@arrivalRate
	,@concurrency)

SELECT * FROM eddsdbo.[UserExperience] WHERE Id = @@IDENTITY