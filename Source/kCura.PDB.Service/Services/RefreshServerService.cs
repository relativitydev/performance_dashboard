namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class RefreshServerService : IRefreshServerService
	{
		public RefreshServerService(ILogger logger, IResourceServerRepository resourceServerRepository)
		{
			this.logger = logger.WithClassName();
			this.resourceServerRepository = resourceServerRepository;
		}

		private readonly ILogger logger;
		private readonly IResourceServerRepository resourceServerRepository;

		/// <summary>
		/// Get All Server List
		/// </summary>
		/// <returns>The list of servers</returns>
		public IList<ResourceServer> GetServerList()
		{
			logger.LogVerbose("GetServerList Called", this.GetType().Name);

			var queryServers = resourceServerRepository.GetAllServers(logger);

			var aggServers = (from i in queryServers
							  group i by new
							  {
								  i.Name,
								  aggCol = AggregateServerTypes(i.ServerType)
							  }

							  into g
							  select new ResourceServer
							  {
								  ArtifactID = g.First().ArtifactID,
								  Name = g.First().Name,
								  ServerInstance = g.First().ServerInstance,
								  IP = g.First().IP,
								  ServerType = g.Key.aggCol
							  }).ToList();

			logger.LogVerbose("GetServerList Called - Success", this.GetType().Name);

			return aggServers;
		}

		/// <summary>
		/// Update [merge] All Server Data in Database
		/// </summary>
		/// <param name="currentServers">The current server list to update</param>
		public void UpdateServerList(IList<ResourceServer> currentServers)
		{
			logger.LogVerbose("UpdateServerList Called", this.GetType().Name);

			try
			{
				logger.LogVerbose("UpdateServerList Called - Creating XML", this.GetType().Name);

				var xml = new XElement(
								"ServerList",
								currentServers
								.Where(server => server.IP.Length <= 15)
								.Select(server =>
									new XElement("Server",
										new XAttribute("Name", string.IsNullOrEmpty(server.ServerInstance) ? server.Name : $"{server.Name}\\{server.ServerInstance}"),
										new XAttribute("IP", server.IP),
										new XAttribute("TypeID", (int)server.ServerType),
										new XAttribute("ArtifactID", server.ArtifactID))));

				logger.LogVerbose($"UpdateServerList Called - Creating XML - Success. XML : {xml}", this.GetType().Name);

				logger.LogVerbose("MergeServerInformation Called", this.GetType().Name);

				resourceServerRepository.MergeServerInformation(xml);

				logger.LogVerbose("MergeServerInformation Called - Success", this.GetType().Name);

			}
			catch (Exception ex)
			{
				logger.LogError(
					string.Format("UpdateServerList Called - Failure. Details: {0}", ex.GetExceptionDetails()),
					this.GetType().Name);
			}

			logger.LogVerbose("UpdateServerList Called - Success", this.GetType().Name);
		}

		/// <summary>
		/// Perform the relativity grouping of server types, to modify, just add 
		/// enumerable to switch statement.
		/// </summary>
		/// <param name="serverType">The server type</param>
		/// <returns>The aggregate server type</returns>
		private static ServerType AggregateServerTypes(ServerType serverType)
		{
			ServerType retServer = serverType;
			switch (serverType)
			{
				case ServerType.WebApi:
				case ServerType.WebBackground:
				case ServerType.Web:
					retServer = ServerType.Web;
					break;
			}

			return retServer;
		}
	}
}
