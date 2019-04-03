namespace kCura.PDB.Data.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Properties;

	public class ServerTestDataRepository : IServerTestDataRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public ServerTestDataRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		/// <summary>
		/// This method needs to create data in the actual Server table.
		/// </summary>
		/// <param name="mockData"></param>
		/// <returns></returns>
		public async Task CreateAsync(IList<MockServer> mockData)
		{
			var servers = mockData.Select(
				s => new Server
				{
					ServerName = s.ServerName,
					ArtifactId = s.ArtifactID,
					ServerTypeId = s.ServerTypeID,
					CreatedOn = s.CreatedOn ?? new DateTime(),
					IgnoreServer = s.IgnoreServer,
					LastServerBackup = s.LastServerBackup
				}).ToList();

			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var createdServers = await servers
										 .Select(async s => await conn.QueryFirstOrDefaultAsync<Server>(Resources.Server_Create, s))
										 .WhenAllStreamed()
										 .ConfigureAwait(false);
				await createdServers.Select(s => conn.ExecuteAsync(Resources.MockServer_Create, new { ServerID = s.ServerId })).WhenAllStreamed()
					.ConfigureAwait(false);
			}
		}

		public async Task ClearAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MockServer_Clear);
			}
		}
	}
}
