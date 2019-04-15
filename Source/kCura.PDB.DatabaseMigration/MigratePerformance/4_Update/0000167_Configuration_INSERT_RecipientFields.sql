/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* This script will take the current notifications recipient setting and apply it to the new set of per-alert recipient settings. *
* After inserting the new settings with the existing recipient value, it will delete the old (unused) global setting.            *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

USE EDDSPerformance
GO

DECLARE @recipient nvarchar(max) = ISNULL(
	(SELECT Value
		FROM eddsdbo.Configuration
		WHERE Section = 'kCura.PDB'
		AND Name = 'NotificationsRecipient'),
	'');

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsBackupDBCCAlerts')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsBackupDBCCAlerts', @recipient, '', 'List of email addresses that should receive backup/DBCC alerts (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsConfigurationChangeAlerts')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsConfigurationChangeAlerts', @recipient, '', 'List of email addresses that should receive configuration change alerts (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsQuarterlyScoreAlerts')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsQuarterlyScoreAlerts', @recipient, '', 'List of email addresses that should receive quarterly score alerts (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsQuarterlyScoreStatus')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsQuarterlyScoreStatus', @recipient, '', 'List of email addresses that should receive quarterly score status notifications (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsSystemLoadForecast')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsSystemLoadForecast', @recipient, '', 'List of email addresses that should receive system load forecast notifications (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsTrustDeliveryAlerts')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsTrustDeliveryAlerts', @recipient, '', 'List of email addresses that should receive Trust delivery notifications (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsUserExperienceForecast')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsUserExperienceForecast', @recipient, '', 'List of email addresses that should receive user experience forecast notifications (separated by commas or semicolons)')
END

IF NOT EXISTS(SELECT TOP 1 Value FROM eddsdbo.Configuration WHERE Section = 'kCura.PDB' AND Name = 'NotificationsRecipientsWeeklyScoreAlerts')
BEGIN
	INSERT INTO eddsdbo.Configuration
	(Section, Name, Value, MachineName, [Description]) VALUES
	('kCura.PDB', 'NotificationsRecipientsWeeklyScoreAlerts', @recipient, '', 'List of email addresses that should receive weekly score alerts (separated by commas or semicolons)')
END

DELETE FROM eddsdbo.Configuration
WHERE Section = 'kCura.PDB'
	AND Name = 'NotificationsRecipient'