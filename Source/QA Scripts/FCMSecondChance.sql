/*
Run this if you've tripped FCM but know how to fix your data/procedures so they're valid again.
This gives you a second chance until full validation happens again (e.g. scores are exported, QoS_TrustScores runs, or
Looking Glass tries to generate new scores). If you fix everything before that happens, FCM won't be tripped again. */

USE EDDSPerformance
GO

IF EXISTS (
	SELECT name, value
	FROM fn_listextendedproperty(default, default, default, default, default, default, default)
	WHERE objtype is null and objname is null and name = 'QoS'
)
BEGIN
	EXEC sys.sp_updateextendedproperty @name = 'QoS', @value = '';
END