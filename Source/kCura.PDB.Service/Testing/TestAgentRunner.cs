namespace kCura.PDB.Service.Testing
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Services;

	public class TestAgentRunner : ITestAgentRunner
	{
		private readonly IMetricSystemManagerService manager;
		private readonly IMetricSystemWorkerService worker;

		private readonly IAppSettingsConfigurationService configurationService;

		public TestAgentRunner(
			IMetricSystemManagerService manager,
			IMetricSystemWorkerService worker,
			IAppSettingsConfigurationService configurationService)
		{
			this.manager = manager;
			this.worker = worker;
			this.configurationService = configurationService;
		}

		public IList<Task> GetAgentExecutionTasks(CancellationToken cancellationToken)
		{
			var managerAgentInRelativity = this.configurationService.ContainsAppSettingsKey(Names.Configuration.ManagerAgentInRelativity) && bool.Parse(this.configurationService.GetAppSetting(Names.Configuration.ManagerAgentInRelativity));
			var workerAgentInRelativity = this.configurationService.ContainsAppSettingsKey(Names.Configuration.WorkerAgentInRelativity) && bool.Parse(this.configurationService.GetAppSetting(Names.Configuration.WorkerAgentInRelativity));

			// Spin up Manager logic
			var tasksToWaitFor = new List<Task>();
			if (managerAgentInRelativity == false)
			{
				tasksToWaitFor.Add(this.manager.Execute(cancellationToken));
			}
			else
			{
				// todo - add task that enables agent in relativity
			}

			// Decide whether to spin up worker logic, or use existing Relativity Worker Agent
			if (workerAgentInRelativity == false)
			{
				tasksToWaitFor.Add(this.worker.Execute(cancellationToken));
			}
			else
			{
				// todo - add task that enables agent in relativity
			}

			// Wait for the logic to execute
			return tasksToWaitFor;
		}
	}
}
