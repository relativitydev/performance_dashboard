namespace kCura.PDB.Service.Agent
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using global::Relativity.Services;
	using global::Relativity.Services.Agent;
	using global::Relativity.Services.ResourceServer;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Extensions;

	public class AgentManagerService : IAgentManagerService
	{
		private readonly IAgentManager agentManager;
		private readonly IAgentRepository agentRepository;
		private readonly ILogger logger;

		public AgentManagerService(IAgentManager agentManager, IAgentRepository agentRepository, ILogger logger)
		{
			this.agentManager = agentManager;
			this.agentRepository = agentRepository;
			this.logger = logger;
		}

		/// <inheritdoc />
		public async Task StartPerformanceDashboardAgentsAsync(IList<int> agentArtifactIds)
		{
			// Ensure there are agents to work with
			if (!agentArtifactIds.Any())
			{
				return;
			}

			await this.ToggleAgentEnabledStatus(true, agentArtifactIds, Defaults.Agent.EnabledStatusRetryCount);
		}
		
		/// <inheritdoc />
		public async Task StartPerformanceDashboardAgentsAsync()
		{
			var agentIds = await this.agentRepository.ReadAgentsAsync(Guids.Agent.CurrentAgentGuids);

			// Ensure there are agents to work with
			if (agentIds.Any())
			{
				await this.StartPerformanceDashboardAgentsAsync(agentIds);
			}
		}

		/// <inheritdoc />
		public async Task<IList<int>> StopPerformanceDashboardAgentsAsync()
		{
			// Get all guid types to turn off
			var enabledAgents = await this.agentRepository.ReadAllEnabledAgentsAsync();

			// Ensure there are agents to work with
			if (!enabledAgents.Any())
			{
				return new List<int>();
			}

			await this.ToggleAgentEnabledStatus(false, enabledAgents, Defaults.Agent.EnabledStatusRetryCount);

			// Return the disabled agents
			return enabledAgents;
		}

		public async Task<IList<int>> CreatePerformanceDashboardAgents(string machineName)
		{
			// Grab the agent server for the given machine name
			var agentServerId = (int?)null;
			try
			{
				agentServerId = await this.GetAgentServerId(machineName);
			}
			catch (InvalidOperationException ex)
			{
				// log error and return
				await this.logger.LogWarningAsync(string.Format(Messages.Exception.PostInstallReadServerId, machineName), ex);
				return new List<int>();
			}

			// Get the agent types that are missing from the environment
			var agentTypesToCreate = await Guids.Agent.CurrentAgentGuids.WhereAsync(async g => await this.agentRepository.ReadAgentWithTypeExists(g) == false);

			// Create them
			return (await agentTypesToCreate.Select(async at =>
				{
					try
					{
						return await this.CreateAgent(agentServerId.Value, at, Names.Agent.FromGuid(at));
					}
					catch (InvalidOperationException ex)
					{
						await this.logger.LogWarningAsync(string.Format(Messages.Exception.PostInstallCreateAgentFailure,
							Names.Agent.FromGuid(at),
							agentServerId.Value));
					}

					return (int?)null; // filter out bad ones
				}).WhenAllStreamed()).Where(ca => ca.HasValue).Select(ca => ca.Value).ToList();
		}

		internal async Task ToggleAgentEnabledStatus(bool enabled, IList<int> agentArtifactIds, int retryCount)
		{
			var success = false;
			var i = 0;
			do
			{
				var agents = await this.QueryAgentsAsync(agentArtifactIds);
				await agents.ForEach(a => a.Enabled = enabled).Select(a => this.agentManager.UpdateSingleAsync(a)).WhenAllStreamed();

				// Wait and recheck they started successfully
				success = await this.WaitAndRecheck(true, agentArtifactIds, Defaults.Agent.EnabledStatusDelaySeconds);
			}
			while (!success && i++ < retryCount);

			if (!success)
			{
				throw new Exception($"Not all agents are in status '{(enabled ? "Enabled" : "Disabled")}'");
			}
		}

		internal async Task<int> CreateAgent(int targetServerArtifactId, Guid agentTypeGuid, string agentName)
		{
			var agentTypeId = await this.GetAgentTypeId(agentTypeGuid);
			var agentDto = new Agent
			{
				Name = agentName,
				AgentType = new AgentTypeRef(agentTypeId),
				Enabled = true,
				Server = new ResourceServerRef { ArtifactID = targetServerArtifactId },
				LoggingLevel = Agent.LoggingLevelEnum.Warning,
				Interval = Defaults.Agent.IntervalFromGuid(agentTypeGuid)
			};

			return await this.agentManager.CreateSingleAsync(agentDto);
		}

		internal async Task<int> GetAgentTypeId(Guid agentTypeGuid)
		{
			var agentTypes = await this.agentManager.GetAgentTypesAsync();
			var agentTypeId = -1;
			try
			{
				agentTypeId = agentTypes.First(at => at.Guids.Contains(agentTypeGuid)).ArtifactID;
			}
			catch
			{
				agentTypeId = agentTypes.First(at =>
					string.Equals(at.Name, Names.Agent.FromGuid(agentTypeGuid), StringComparison.CurrentCultureIgnoreCase)).ArtifactID;
			}

			return agentTypeId;
		}

		internal async Task<int> GetAgentServerId(string machineName)
		{
			var agentServers = await this.agentManager.GetAgentServersAsync();
			return agentServers.First(s => string.Equals(s.Name, machineName, StringComparison.CurrentCultureIgnoreCase)).ArtifactID;
		}

		/// <summary>
		/// Use AgentManager to query for AgentRDOs based on given ArtifactIds
		/// </summary>
		/// <param name="artifactIds"></param>
		/// <returns></returns>
		internal async Task<IList<Agent>> QueryAgentsAsync(IList<int> artifactIds)
		{
			// Ensure there are agents to work with
			if (artifactIds.Any() == false)
			{
				return new List<Agent>();
			}

			var query = new Query
			{
				Condition = new WholeNumberCondition(
									"ArtifactID",
									NumericConditionEnum.In,
									artifactIds.ToList()).ToQueryString()
			};
			var agentQueryResults = await this.agentManager.QueryAsync(query);
			agentQueryResults.ThrowIfUnsuccessful();

			return agentQueryResults.Results.Select(r => r.Artifact).ToList();
		}

		internal async Task<bool> WaitAndRecheck(bool enabled, IList<int> agentArtifactIds, int delaySeconds)
		{
			// Ensure there are agents to work with
			if (agentArtifactIds.Any() == false)
			{
				return true;
			}
			
			// Wait for 10 seconds
			await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
			var agentsEnabled = await this.QueryAgentsAsync(agentArtifactIds);
			return agentsEnabled.All(a => a.Enabled == enabled);
		}
	}
}
