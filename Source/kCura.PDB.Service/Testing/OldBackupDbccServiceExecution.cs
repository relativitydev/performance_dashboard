namespace kCura.PDB.Service.Testing
{
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Testing.Services;

	/// <summary>
    /// Execution class for the old backup/dbcc monitor system.
    /// Executes for 
    /// </summary>
    public class OldBackupDbccServiceExecution : IServiceExecution
    {
        private readonly ISqlServerRepository sqlServerRepository;
		private readonly IAgentService agentService;

        public OldBackupDbccServiceExecution(ISqlServerRepository sqlServerRepository, IAgentService agentService)
        {
            this.sqlServerRepository = sqlServerRepository;
	        this.agentService = agentService;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
	        var agentId = this.agentService.AgentID;

			// Execute the process
			var results = this.sqlServerRepository.ExecuteBackupDBCCMonitor(agentId, Names.Database.BackupAndDBCCMonLauncherSproc_Test);

			// Return
			return Task.CompletedTask;
        }
	}
}
