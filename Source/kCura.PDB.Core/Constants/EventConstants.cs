namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models;

	public static class EventConstants
	{
		public const int DefaultEventDelay = 30;
		public const int DefaultMaxEventDelay = 15 * 60;
		public const int DefaultManagerRunInterval = 10 * 60; // 10 minutes
		public const int DefaultManagerMeterReportTimeout = 60;
		public const int MinManagerRunInterval = 1;
		public const int MinManagerMeterReportTimeout = 5;
		public const int DefaultEnqueueTasksInterval = 500;
		public const int MinEnqueueTasksInterval = 100;
		public const int DefaultCreateBootstrapEventsInterval = 10;
		public const int DefaultResolveOrphanedEventsInterval = 120; // seconds
		public const int MinResolveOrphanedEventsInterval = 1; // seconds
		public const int MinCreateBoostrapEventsInterval = 1;
		public const long DefaultEventsToEnqueue = 250;
		public const long MinEventsToEnqueue = 1;
		public const long DefaultErroredEventsToEnqueue = 1000;

		public static readonly IList<EventSourceType> AllEventTypes = Enum.GetValues(typeof(EventSourceType)).Cast<EventSourceType>().ToList();

		public static readonly IList<EventStatus> ActiveEventStatuses = new[]
		{
			EventStatus.PendingHangfire,
			EventStatus.InProgress,
		};

		public static readonly IList<EventSourceType> HourBootstrapEvents = new[]
		{
			EventSourceType.CheckForHourPrerequisites
		};

		public static readonly IList<EventSourceType> PrerequisiteEvents = new[]
		{
			EventSourceType.CheckAllPrerequisitesComplete,
			EventSourceType.CheckForHourPrerequisites,
			EventSourceType.DeployServerDatabases,
			EventSourceType.StartQosDatabaseDeployment,
			EventSourceType.StartPrerequisites,
			EventSourceType.StartMigrateEvents,
			EventSourceType.IdentifyIncompleteHours,
			EventSourceType.CancelEvents,
			EventSourceType.CancelHour,
			EventSourceType.CompletePrerequisites,
		};

		public static readonly IList<EventSourceType> SingletonEventTypes = new[]
		{
			EventSourceType.StartHour,
			EventSourceType.ProcessRecoverabilityForServer,
			EventSourceType.CheckForHourPrerequisites,
			EventSourceType.FindNextCategoriesToScore,
		};

		public static readonly IList<EventSourceType> LoopEventTypes = new[]
		{
			EventSourceType.CheckMetricDataIsReadyForDataCollection,
		};

		public static IList<EventSourceType> GetNextEvents(EventSourceType eventType) => new[]
		{
			new Hierarchy(EventSourceType.CheckForHourPrerequisites), // event task returns it's own next events in event result

			EventSourceType.CreateAuditProcessingBatches
						.WithNext(EventSourceType.ProcessAuditBatches),

			EventSourceType.CreateNextHour
				.WithNext(EventSourceType.StartHour
					.WithNext(EventSourceType.CreateCategoriesForHour
						.WithNext(EventSourceType.CreateCategoryScoresForCategory
							.WithNext(EventSourceType.CreateMetricDatasForCategoryScores
								.WithNext(EventSourceType.CheckSamplingPeriodForMetricData // event task returns it's own next events in event result
									.WithNext(EventSourceType.StartPrerequisitesForMetricData // event task returns it's own next events in event result
										.WithNext(EventSourceType.CheckMetricDataIsReadyForDataCollection // event task returns it's own next events in event result
											.WithNext(EventSourceType.CollectMetricData // event task returns it's own next events in event result
												.WithNext(EventSourceType.ScoreMetricData))))))))),

			EventSourceType.FindNextCategoriesToScore
				.WithNext(EventSourceType.ScoreCategoryScore
					.WithNext(EventSourceType.CompleteCategory
						.WithNext(EventSourceType.CheckIfHourReadyToScore
							.WithNext(EventSourceType.ScoreHour
								.WithNext(
									EventSourceType.SendScoreAlerts
										.WithNext(EventSourceType.CompleteHour),
									EventSourceType.HourCleanup
										.WithNext(EventSourceType.HourServerCleanup
											.WithNext(EventSourceType.CompleteHour))))))),

			EventSourceType.StartPrerequisites
				.WithNext(
					EventSourceType.StartQosDatabaseDeployment
						.WithNext(EventSourceType.DeployServerDatabases
							.WithNext(EventSourceType.CheckAllPrerequisitesComplete)),
					EventSourceType.StartMigrateEvents
						.WithNext(EventSourceType.CancelEvents
							.WithNext(EventSourceType.IdentifyIncompleteHours
								.WithNext(EventSourceType.CancelHour
									.WithNext(EventSourceType.CheckAllPrerequisitesComplete))))),

			EventSourceType.CheckAllPrerequisitesComplete
				.WithNext(EventSourceType.CompletePrerequisites)


		}
		.FindNextTypes(eventType);

		private static Hierarchy WithNext(this EventSourceType type, params EventSourceType[] nextTypes) => new Hierarchy(type).WithNext(nextTypes.Select(nt => new Hierarchy(nt)).ToArray());

		private static Hierarchy WithNext(this EventSourceType type, params Hierarchy[] nextTypes) => new Hierarchy(type).WithNext(nextTypes);

		private static Hierarchy WithNext(this Hierarchy hierarchy, params Hierarchy[] eventHierarchies)
		{
			hierarchy.Next = eventHierarchies;
			return hierarchy;
		}

		private static IList<EventSourceType> FindNextTypes(this IList<Hierarchy> hierarchies, EventSourceType searchType) =>
			hierarchies.Any(h => h.Type == searchType)
				? hierarchies.First(h => h.Type == searchType).Next.Select(n => n.Type).ToList()
				: hierarchies.SelectMany(n => n.Next.FindNextTypes(searchType)).ToList();

		private class Hierarchy
		{
			public Hierarchy(EventSourceType type)
			{
				this.Type = type;
			}

			public EventSourceType Type { get; }

			public IList<Hierarchy> Next { get; set; } = new Hierarchy[0];
		}
	}
}