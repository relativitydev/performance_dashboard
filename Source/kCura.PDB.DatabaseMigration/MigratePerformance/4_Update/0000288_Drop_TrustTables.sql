-- EDDSPerformance

-- Remove deprecated tables
IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TrustSendLog')
BEGIN
	DROP TABLE eddsdbo.TrustSendLog
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TrustWeeklyScores')
BEGIN
	DROP TABLE eddsdbo.TrustWeeklyScores
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'TrustWeeks')
BEGIN
	DROP TABLE eddsdbo.TrustWeeks
END


-- Remove deprecated configs
DELETE FROM [eddsdbo].[Configuration]
WHERE Name in (
	'AutomaticTrustSync',
	'NotificationsRecipientsTrustDeliveryAlerts',
	'SendTrustDeliveryNotifications',
	'FCMResetActive',
	'AssemblyFileVersion',
	'AssemblyName')