namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Services;

	public abstract class BaseProcessControlTask
	{
		protected BaseProcessControlTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
		{
			this.logger = logger;
			SqlRepo = sqlRepo;
			AgentId = agentId;
		}

		protected BaseProcessControlTask(ISqlServerRepository sqlRepo, int agentId)
		{
			SqlRepo = sqlRepo;
			AgentId = agentId;
		}

		protected readonly ISqlServerRepository SqlRepo;
		protected readonly int AgentId;
		protected string initialDatabase = Names.Database.EddsQoS;
		private readonly ILogger logger;

		private string name = null;
		public string Name => name ?? (name = GetTaskName());

		protected List<Server> GetServers()
		{
			return this.SqlRepo.PerformanceServerRepository.ReadAllActiveAsync()
				.GetAwaiter().GetResult()
				.OrderByDescending(s => s.CreatedOn)
				.ToList();
		}

		protected void ExecuteForServers(Action<Server> executeServer)
		{
			var srvs = this.GetServers();
			foreach (var server in srvs)
			{
				Log("Running for server: {0}", server.ServerName);
				executeServer(server);
			}
		}

		protected void Log(string msg, params object[] args)
		{
			if (logger == null)
			{
				throw new Exception("Log: Logger wasn't configured for this task.");
			}
				
			logger.LogVerbose(string.Format(Name + " - " + msg, args), Name);
		}

		protected void LogError(string msg, params object[] args)
		{
			if (logger == null)
			{
				throw new Exception("LogError: Logger wasn't configured for this task.");
			}

			logger.LogError(string.Format(Name + " - " + msg, args), Name);
		}

		protected void LogWarning(string msg, params object[] args)
		{
			if (logger == null)
			{
				throw new Exception("LogError: Logger wasn't configured for this task.");
			}

			logger.LogWarning(string.Format(Name + " - " + msg, args), Name);
		}

		private string GetTaskName()
		{
			var taskType = this.GetType();

			if (taskType.IsDefined<DescriptionAttribute>())
			{
				var desc = taskType.Get<DescriptionAttribute>();
				return desc.Description;
			}

			return taskType.Name;
		}
	}
}
