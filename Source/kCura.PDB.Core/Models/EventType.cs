namespace kCura.PDB.Core.Models
{
	public enum EventSourceType
	{
		/*
			The following event source types are now obsolete
			CreateMetric = 1,
			CreateMetricData = 2,
			CreateCategory = 5,
			CreateCategoryScore = 6,
			CreateHour = 8,
		*/

		// Hour events 100-199
		ScoreHour = 9,
		CreateNextHour = 100,
		CheckIfHourReadyToScore = 101,
		HourCleanup = 102,
		HourServerCleanup = 103,
		StartHour = 104,
		CompleteHour = 105,

		// Metric events 200 - 299
		CollectMetricData = 3,
		ScoreMetricData = 4,
		CheckMetricDataIsReadyForDataCollection = 202,
		CheckSamplingPeriodForMetricData = 203,
		StartPrerequisitesForMetricData = 204,
		CreateMetricDatasForCategoryScores = 205,

		// Category Events 300-399
		ScoreCategoryScore = 7,
		CreateCategoriesForHour = 300,
		CreateCategoryScoresForCategory = 301,
		FindNextCategoriesToScore = 304,
		CompleteCategory = 310,

		// Batching events 1000-1099
		CreateAuditProcessingBatches = 1001,
		ProcessAuditBatches = 1011,

		// Recoverability/Integrity process gaps
		ProcessRecoverabilityForServer = 1101,

		// Misc/Other 2000-3000
		SendScoreAlerts = 2000,

		// Prerequisites events
		CheckForHourPrerequisites = 7000,
		DeployServerDatabases = 7002,
		CheckAllPrerequisitesComplete = 7003,
		StartQosDatabaseDeployment = 7004,
		StartPrerequisites = 7005,
		CompletePrerequisites = 7006,

		StartMigrateEvents = 7101,
		CancelEvents = 7102,
		IdentifyIncompleteHours = 7103,
		CancelHour = 7104,

		// Test / No-op events
		NoOpSimple = 99901,
		FailsThenSucceeds = 99902,
	}
}
