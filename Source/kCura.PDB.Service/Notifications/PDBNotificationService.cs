namespace kCura.PDB.Service.Notifications
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.DatabaseDeployment;

	public class PdbNotificationService : IPDBNotificationService
	{
		private readonly IEventRepository eventRepository;
		private readonly IServerRepository serverRepository;
		private readonly IPDBNotificationRepository pdbNotificationRepository;

		public PdbNotificationService(ISqlServerRepository sqlServerRepository)
		{
			this.eventRepository = sqlServerRepository.EventRepository;
			this.serverRepository = sqlServerRepository.PerformanceServerRepository;
			this.pdbNotificationRepository = sqlServerRepository.PDBNotificationRepository;
		}

		/// <summary>
		/// Gets the next most sever notification
		/// </summary>
		/// <returns>Result notification or null if none.</returns>
		public PDBNotification GetNext()
		{
			var checks = new Func<PDBNotification>[]
			{
				this.GetDatabaseDeploymentFailureAlert,
				this.GetProccessControlNotification,
				this.GetAgentsAlert,
				this.GetSqlAgentsAlert,
			};

			//check critical checks before warning checks
			return GetNext(checks);
		}

		public PDBNotification GetProccessControlNotification()
		{
			var datatable = this.pdbNotificationRepository.GetFailingProcessControls();
			if (datatable.Rows.Count <= 0)
				return null;

			var totalFailingProcessControls = datatable.Rows.Count;

			var failingProcessControls = (from DataRow d in datatable.Rows
										  select new
										  {
											  ProcessControlID = d.Field<int>("ProcessControlID"),
											  ProcessTypeDesc = d.Field<string>("ProcessTypeDesc"),
										  }).ToList();

			var notification = new PDBNotification
			{
				Type = NotificationType.Critical,
				Message = string.Format(
					Messages.Notification.ProcessControlFailed,
					totalFailingProcessControls,
					string.Join(", ", failingProcessControls.Select(fpc => fpc.ProcessControlID + "-" + fpc.ProcessTypeDesc)))
			};

			return notification;
		}

		public PDBNotification GetAgentsAlert()
		{
			var datatable = this.pdbNotificationRepository.GetAgentsAlert();
			if (datatable.Rows.Count <= 0)
				return null;

			var disabledAgents = (from DataRow d in datatable.Rows
								  select new
								  {
									  NumberOfAgents = d.Field<int>("NumberOfAgents"),
									  AgentName = d.Field<string>("Name"),
								  }).Where(da => da.NumberOfAgents == 0 && !da.AgentName.Contains("Trust")).ToList();
			// Keeping the trust exception since the trust agent is not fully removed yet

			if (disabledAgents.Any() == false)
				return null;

			var notification = new PDBNotification
			{
				Type = NotificationType.Warning,
				Message = string.Format(Messages.Notification.NoAgentsEnabled, string.Join(", ", disabledAgents.Select(a => a.AgentName)))
			};
			return notification;
		}

		public PDBNotification GetSqlAgentsAlert()
		{
			var secondsSinceLastRecord = this.pdbNotificationRepository.GetSecondsSinceLastAgentHistoryRecord();

			if (secondsSinceLastRecord.HasValue == false || secondsSinceLastRecord > 600)
			{
				var notification = new PDBNotification()
				{
					Type = NotificationType.Warning,
					Message = Messages.Notification.NoSqlAgentsReporting
				};
				return notification;
			}

			return null;
		}

		public PDBNotification GetDatabaseDeploymentFailureAlert()
		{
			return Task.Run(this.GetDatabaseDeploymentFailureAlertAsync).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Creates notification if database deployment failed.
		/// Notification severity is determined by whether the application is stuck in prerequisites mode.
		/// </summary>
		/// <returns>Notification</returns>
		public async Task<PDBNotification> GetDatabaseDeploymentFailureAlertAsync()
		{
			// TODO make sure that async results don't deadlock web server or add non-async repo methods
			var databaseDeplymentService = new DatabaseDeploymentService(this.eventRepository, this.serverRepository);
			var eventSystemState = await this.eventRepository.ReadEventSystemStateAsync();

			return (await databaseDeplymentService.FindAnyFailedDeployments())
				? new PDBNotification()
				{
					Type = eventSystemState == EventSystemState.Prerequisites ? NotificationType.Critical : NotificationType.Warning,
					Message = Messages.Notification.DeploymentFailure
				}
				: null;
		}

		/// <summary>
		/// runs check methods until it finds a check that doesn't return null or runs all checks
		/// </summary>
		/// <param name="checks">The list of notification checks to perform</param>
		/// <returns>Notification for first check that succeeds or returns null</returns>
		private static PDBNotification GetNext(IList<Func<PDBNotification>> checks)
		{
			return checks
				.Select(check => check())
				.FirstOrDefault(check => check != null);
		}
	}
}
