namespace kCura.PDB.Service.Queuing
{
	using System;
	using System.Drawing.Text;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing;
	using kCura.PDB.Core.Models;

	public class EventRouter : IEventRouter
	{
		public EventRouter(IEventRunner eventRunner)
		{
			this.eventRunner = eventRunner;
		}

		private readonly IEventRunner eventRunner;

		/// <summary>
		/// Routes the event to the service and action to take on the event
		/// </summary>
		/// <param name="evnt">The event t take an action on</param>
		/// <returns>The result from the action taken on the event</returns>
		public Task<EventResult> RouteEvent(Event evnt)
		{
			// TODO: make this dynamic
			switch (evnt.SourceType)
			{
				case EventSourceType.CheckMetricDataIsReadyForDataCollection:
					return this.eventRunner.HandleEvent<IMetricTask>(t => t.CheckMetricReadyForDataCollection(evnt.SourceId.Value), evnt);

				case EventSourceType.CheckIfHourReadyToScore:
					return this.eventRunner.HandleEvent<IHourTask>(t => t.CheckIfHourReadyToScore(evnt.HourId.Value), evnt);

				case EventSourceType.CollectMetricData:
					return this.eventRunner.HandleEvent<IMetricTask>(t => t.CollectMetricData(evnt.SourceId.Value), evnt);

				case EventSourceType.CreateAuditProcessingBatches:
					return this.eventRunner.HandleEvent<IAuditServerBatcher>(t => t.CreateServerBatches(evnt.SourceId.Value), evnt);

				case EventSourceType.ProcessAuditBatches:
					return this.eventRunner.HandleEvent<IAuditBatchProcessor>(t => t.ProcessBatch(evnt.SourceId.Value), evnt);

				case EventSourceType.CreateNextHour:
					return this.eventRunner.HandleEvent<IHourService>(s => s.CreateNextHours(), evnt);

				case EventSourceType.CreateCategoriesForHour:
					return this.eventRunner.HandleEvent<ICategoryService>(s => s.CreateCategoriesForHour(evnt.SourceId.Value), evnt);

				case EventSourceType.CreateCategoryScoresForCategory:
					return this.eventRunner.HandleEvent<ICategoryService>(s => s.CreateCategoryScoresForCategory(evnt.SourceId.Value), evnt);

				case EventSourceType.CreateMetricDatasForCategoryScores:
					return this.eventRunner.HandleEvent<IMetricService>(s => s.CreateMetricDatasForCategoryScores(evnt.SourceId.Value), evnt);

				case EventSourceType.CheckForHourPrerequisites:
					return this.eventRunner.HandleEvent<IHourPrerequisitesService>(t => t.CheckForPrerequisites(), evnt);

				case EventSourceType.DeployServerDatabases:
					return this.eventRunner.HandleEvent<IQosDatabaseDeployer>(t => t.ServerDatabaseDeployment(evnt.SourceId.Value), evnt);

				case EventSourceType.CheckAllPrerequisitesComplete:
					return this.eventRunner.HandleEvent<IHourPrerequisitesService>(t => t.CheckAllPrerequisitesComplete(), evnt);

				case EventSourceType.CompleteHour:
					return this.eventRunner.HandleEvent<IHourService>(t => t.CompleteHour(evnt.SourceId ?? evnt.HourId.Value), evnt);

				case EventSourceType.ScoreCategoryScore:
					return this.eventRunner.HandleEvent<ICategoryScoringTask>(t => t.ScoreCategory(evnt.SourceId.Value), evnt);

				case EventSourceType.CompleteCategory:
					return this.eventRunner.HandleEvent<ICategoryCompleteTask>(t => t.CompleteCategory(evnt.SourceId.Value), evnt);

				case EventSourceType.ScoreHour:
					return this.eventRunner.HandleEvent<IHourTask>(t => t.ScoreHour(evnt.SourceId.Value), evnt);

				case EventSourceType.ScoreMetricData:
					return this.eventRunner.HandleEvent<IMetricTask>(t => t.ScoreMetric(evnt.SourceId.Value), evnt);

				case EventSourceType.SendScoreAlerts:
					return this.eventRunner.HandleEvent<ISendScoreAlertsService>(t => t.SendNotifications(evnt.SourceId.Value), evnt);

				case EventSourceType.FindNextCategoriesToScore:
					return this.eventRunner.HandleEvent<ICategoryScoringTask>(t => t.FindNextCategoriesToScore(), evnt);

				case EventSourceType.HourCleanup:
					return this.eventRunner.HandleEvent<IHourCleanupLogic>(t => t.CleanupForHour(evnt.SourceId.Value), evnt);

				case EventSourceType.HourServerCleanup:
					return this.eventRunner.HandleEvent<IHourCleanupLogic>(t => t.CleanupQosTables(evnt.SourceId.Value), evnt);

				case EventSourceType.CheckSamplingPeriodForMetricData:
					return this.eventRunner.HandleEvent<IMetricTask>(t => t.CheckSamplingPeriodForMetricData(evnt.SourceId.Value), evnt);

				case EventSourceType.StartHour:
					return this.eventRunner.HandleEvent<IHourService>(t => t.StartHour(evnt.SourceId.Value), evnt);

				case EventSourceType.StartPrerequisitesForMetricData:
					return this.eventRunner.HandleEvent<IMetricTask>(t => t.StartPrerequisitesForMetricData(evnt.SourceId.Value), evnt);

				case EventSourceType.IdentifyIncompleteHours:
					return this.eventRunner.HandleEvent<IHourMigrationService>(t => t.IdentifyIncompleteHours(), evnt);

				case EventSourceType.CancelHour:
					return this.eventRunner.HandleEvent<IHourMigrationService>(t => t.CancelHour(evnt.SourceId.Value), evnt);

				case EventSourceType.CancelEvents:
					return this.eventRunner.HandleEvent<IHourMigrationService>(t => t.CancelEvents(), evnt);

				case EventSourceType.StartQosDatabaseDeployment:
					return this.eventRunner.HandleEvent<IQosDatabaseDeployer>(t => t.StartQosDatabaseDeployment(), evnt);

				case EventSourceType.CompletePrerequisites:
					return this.eventRunner.HandleEvent<IHourPrerequisitesService>(t => t.CompletePrerequisites(), evnt);

				case EventSourceType.FailsThenSucceeds:
					return this.eventRunner.HandleEvent<INoOpTask>(t => t.FailsThenSucceeds(evnt.Id), evnt);

				case EventSourceType.ProcessRecoverabilityForServer:
					return this.eventRunner.HandleEvent<IServerRecoverabilityProcessor>(t => t.ProcessRecoverabilityForServer(evnt.SourceId.Value), evnt);

				case EventSourceType.StartPrerequisites:
				case EventSourceType.StartMigrateEvents:
				case EventSourceType.NoOpSimple:
					return Task.FromResult(EventResult.Continue);

				default:
					throw new Exception($"Cannot create task from event. Unsupported event source type. {evnt.SourceTypeId}");
			}
		}
	}
}
