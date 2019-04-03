namespace kCura.PDB.Service.Agent
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Logging;

	public class DatabaseLogger : GenericLogger, ILogger
	{
		private readonly ILogRepository logRepository;
		private readonly IEventRepository eventRepository;
		private readonly IAgentService agentService;
		private readonly ILogService logService;
		private readonly ILogContext logContext;

		public DatabaseLogger(ILogRepository logRepository, ILogService logService, ILogContext logContext, IEventRepository eventRepository, IAgentService agentService)
		{
			this.logRepository = logRepository;
			this.agentService = agentService;
			this.logService = logService;
			this.logContext = logContext;
			this.eventRepository = eventRepository;
		}

		public DatabaseLogger(ILogRepository logRepository, ILogService logService, ILogContext logContext, IEventRepository eventRepository)
			: this(logRepository, logService, logContext, eventRepository, null)
		{
		}

		public DatabaseLogger(ILogRepository logRepository, ILogService logService, IAgentService agentService)
			: this(logRepository, logService, null, null, agentService)
		{
		}

		public DatabaseLogger(ILogRepository logRepository, ILogService logService)
			: this(logRepository, logService, null, null, null)
		{
		}

		public override Task LogAsync(int level, string message, params string[] categories) =>
			this.LogWithIdAsync(level, message, categories);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an async method.")]
		internal async Task LogWithIdAsync(int level, string message, params string[] categories)
		{
			if (this.logService.ShouldLog(level, categories))
			{
				var module = string.Join(", ", categories).Truncate(100);
				var logEntry = new LogEntry
				{
					Module = module,
					AgentId = this.agentService?.AgentID ?? -1,
					TaskCompleted = message.Truncate(500),
					OtherVars = message,
					LogLevel = level
				};

				await this.logRepository.CreateAsync(logEntry)
					.PipeAsync(CreateEventLogAsync);
			}
		}

		internal void CreateEventLog(int logId)
		{
			if (this.eventRepository != null && this.logContext != null)
			{
				var evnt = this.logContext.Event;
				if (evnt != null)
					this.eventRepository.CreateEventLog(evnt.Id, logId);
			}
		}

		internal async Task CreateEventLogAsync(int logId)
		{
			if (this.eventRepository != null && this.logContext != null)
			{
				var evnt = this.logContext.Event;
				if (evnt != null)
					await this.eventRepository.CreateEventLogAsync(evnt.Id, logId);
			}
		}

		protected override void Log(int level, string message, params string[] categories)
		{
			if (this.logService.ShouldLog(level, categories))
			{
				var module = string.Join(", ", categories).Truncate(100);
				var logEntry = new LogEntry
				{
					Module = module,
					AgentId = this.agentService?.AgentID ?? -1,
					TaskCompleted = message.Truncate(500),
					OtherVars = message,
					LogLevel = level
				};

				this.logRepository.Create(logEntry)
					.Pipe(CreateEventLog);
			}
		}
	}
}
