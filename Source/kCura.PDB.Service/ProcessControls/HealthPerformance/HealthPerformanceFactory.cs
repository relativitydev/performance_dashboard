namespace kCura.PDB.Service.ProcessControls.HealthPerformance
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class HealthPerformanceFactory : IPerformanceTask
	{
		public IAgentService AgentService { get; set; }
		public ISqlServerRepository SqlService { get; set; }
		public ILogger Logger { get; set; }

		private Server GetServerClaim()
		{
			var servers = new List<Server>();
			Server server = null;

			Logger.LogVerbose("GetServerClaim Called", GetType().Name);

			try
			{

				servers = SqlService.PerformanceServerRepository.ReadAllActive().OrderBy(s => s.LastChecked).ToList();

				//Claim the first available server and start working on it
				foreach (var s in servers)
				{
					if (SqlService.ClaimServer(s.ServerId, AgentService.AgentID.ToString()))
					{
						server = s;
						break;
					}
				}

				if (server == null)
					Logger.LogVerbose("GetServerClaim Called - Success. No outstanding work to be claimed.", this.GetType().Name);
				else
					Logger.LogVerbose(string.Format("GetServerClaim Called - Success. Claimed Server: {0} ({1})", server.ServerName, server.ServerId), this.GetType().Name);
			}
			catch (Exception ex)
			{
				Logger.LogError(string.Format("GetServerClaim Called - Failure. Details: {0}", ex.Message), this.GetType().Name);
			}

			return server;
		}

		public void GetPerformanceMetrics()
		{
			var server = GetServerClaim();

			if (server == null)
			{
				Logger.LogVerbose("GetPerformanceMetrics skipped. No servers were claimed.", this.GetType().Name);
				return;
			}

			Logger.LogVerbose("GetPerformanceMetrics Called", this.GetType().Name);

			var withDelay = false;
			try
			{
				var diskTask = new DiskHealthPerformanceTask { Logger = Logger, SqlService = SqlService };
				var ramTask = new RamHealthPerformanceTask { Logger = Logger, SqlService = SqlService };
				var cupTask = new CPUHealthPerformanceTask { Logger = Logger, SqlService = SqlService };
				var sqlTask = new SQLServerPerformanceTask { Logger = Logger, SqlService = SqlService };

				//SQL Server Performance Task
				if (server.ServerTypeId == 3)
				{
					Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling SQLServerPerformanceTask for '{0}'", server.ServerName),
							this.GetType().Name);
					sqlTask.ProcessServer(server);
					Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling SQLServerPerformanceTask for '{0}' - Success", server.ServerName),
													this.GetType().Name);
				}

				Logger.LogVerbose("Calling SQL SavePerformanceMetrics", this.GetType().Name);
				sqlTask.SavePerformanceMetrics();
				Logger.LogVerbose("Calling SQL SavePerformanceMetrics - Success", this.GetType().Name);

				//Disk Health Performance Task
				if (server.ServerTypeId == 1 || server.ServerTypeId == 3 || server.ServerTypeId == 22)
				{
					Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling DiskHealthPerformanceTask for '{0}'", server.ServerName),
													this.GetType().Name);
					diskTask.ProcessServer(server);
					Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling DiskHealthPerformanceTask for '{0}' - Success", server.ServerName),
													this.GetType().Name);
				}

				Logger.LogVerbose("Calling Disk SavePerformanceMetrics", this.GetType().Name);
				diskTask.SavePerformanceMetrics();
				Logger.LogVerbose("Calling Disk SavePerformanceMetrics - Success", this.GetType().Name);

				//RAM Health Performance Task
				Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling RamHealthPerformanceTask for '{0}'", server.ServerName),
											this.GetType().Name);
				ramTask.ProcessServer(server);
				Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling RamHealthPerformanceTask for '{0}' - Success", server.ServerName),
											this.GetType().Name);

				Logger.LogVerbose("Calling RAM SavePerformanceMetrics", this.GetType().Name);
				ramTask.SavePerformanceMetrics();
				Logger.LogVerbose("Calling RAM SavePerformanceMetrics - Success", this.GetType().Name);

				//CPU Health Performance Task
				Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling CPUHealthPerformanceTask for '{0}'", server.ServerName),
											this.GetType().Name);
				cupTask.ProcessServer(server);
				Logger.LogVerbose(string.Format("GetPerformanceMetrics Called - Calling CPUHealthPerformanceTask for '{0}' - Success", server.ServerName),
											this.GetType().Name);

				Logger.LogVerbose("Calling CPU SavePerformanceMetrics", this.GetType().Name);
				cupTask.SavePerformanceMetrics();
				Logger.LogVerbose("Calling CPU SavePerformanceMetrics - Success", this.GetType().Name);
			}
			catch (Exception ex)
			{
				Logger.LogWarning(string.Format("GetPerformanceMetrics Called - Failure. Details: {0}", ex.Message), this.GetType().Name);
				withDelay = true; //This will stop agents from claiming this server for the next hour
			}
			finally
			{
				SqlService.UnclaimServer(server.ServerId, AgentService.AgentID.ToString(), withDelay);
			}
		}
	}
}
