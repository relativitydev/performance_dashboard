namespace kCura.PDB.Service.DatabaseDeployment
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.DatabaseDeployment;
	using kCura.PDB.Core.Interfaces.Repositories;

	public class QosDatabaseDeployer : IQosDatabaseDeployer
	{
		private readonly IServerRepository serverRepository;
		private readonly IDatabaseDeployer databaseDeployer;

		public QosDatabaseDeployer(
			IServerRepository serverRepository,
			IDatabaseDeployer databaseDeployer)
		{
			this.serverRepository = serverRepository;
			this.databaseDeployer = databaseDeployer;
		}

		public async Task ServerDatabaseDeployment(int serverId)
		{
			var server = await this.serverRepository.ReadAsync(serverId);

			// Deploy to the server
			this.databaseDeployer.DeployQos(server);

			// set the server deployed
			server.IsQoSDeployed = true;
			await this.serverRepository.UpdateAsync(server);
		}

		public async Task<IList<int>> StartQosDatabaseDeployment()
		{
			var serversPendingQosDeployment = await this.serverRepository.ReadServerPendingQosDeploymentAsync();
			return serversPendingQosDeployment.Select(s => s.ServerId).ToList();
		}
	}
}
