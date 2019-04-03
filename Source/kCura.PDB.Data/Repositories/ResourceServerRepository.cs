namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Xml.Linq;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Services;

	[Obsolete("Use ServerRepository")]
	public class ResourceServerRepository : BaseRepository, IResourceServerRepository
	{
		public ResourceServerRepository(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{
		}

		public IEnumerable<ResourceServer> ReadResourceServers()
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsConnection())
			{
				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Properties.Resources.ReadResourceServers);
				return GetResourceServersFromDataSet(data);
			}
		}

		public IEnumerable<ResourceServer> GetResourceServersFromDataSet(DataSet data)
		{
			if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow record in data.Tables[0].Rows)
				{
					yield return new ResourceServer()
					{
						ArtifactID = record.GetField<Int32>("ArtifactID"),
						Name = record.GetField<String>("ServerName"),
						ServerType = GetResourceServerType(record.GetField<String>("ServerType")),
						Url = record.GetField<String>("URL")
					};
				}
			}
		}

		[Obsolete("Use ServerRespository")]
		public IEnumerable<String> ReadFileServers()
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetEddsConnection())
			{
				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Properties.Resources.ReadFileServers);

				if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow record in data.Tables[0].Rows)
					{

						yield return record.GetField<String>("Name");
					}
				}
			}
		}

		/// <summary>
		/// Get list of the server
		/// </summary>
		/// <param name="authInfo">Authentication Information</param>
		/// <returns>Return list of the server after resolving server host addresses</returns>
		[Obsolete("Use ServerRepository")]
		public IList<ResourceServer> GetAllServers(ILogger logger)
		{
			var servers = ReadResourceServers().ToList();
			// Get File server and File location path
			servers.AddRange(GetFileServers(logger));
			// Update the server names
			UpdateAllServerNames(servers);
			// Get IP addresses for servers
			var serversWithIps = GetAllServersWithIps(servers, logger);

			return serversWithIps;
		}

		public void UpdateAllServerNames(IList<ResourceServer> servers)
		{
			servers
				.Where(s => string.IsNullOrEmpty(s.Url) == false
							&& (s.ServerType == ServerType.Invariant || s.ServerType == ServerType.Analytics)
							&& s.Url.Split('/', ':').Length >= 4)
				.ForEach(s => s.Name = s.Url.Split('/', ':')[3]);
			servers
				.Where(s => string.IsNullOrEmpty(s.Url) == false
							&& s.ServerType == ServerType.Invariant
							&& s.Url.Split('/', ':').Length == 1)
				.ForEach(s => s.Name = s.Url);
			servers
				.Where(s => string.IsNullOrEmpty(s.Url) == false
							&& s.ServerType == ServerType.CacheLocation)
				.ForEach(s => s.Name = s.Url.Split('\\')[2]);

			servers
				.Where(s => s.ServerType != ServerType.Database && s.Name.Contains('\\'))
				.ForEach(s => s.Name = s.Name.Split('\\')[0])
				.ForEach(s => s.ServerInstance = s.Name.Split('\\')[1]);
		}

		[Obsolete("Use ServerRepository")]
		public IEnumerable<ResourceServer> GetFileServers(ILogger logger)
		{
			try
			{
				var fileServerNames = ReadFileServers().ToList();

				return fileServerNames
					.Where(fsm => !string.IsNullOrWhiteSpace(fsm))
					.Select(fsm => fsm.Split('\\')[2])
					.Select(fsm => new ResourceServer()
					{
						ArtifactID = -1,
						Name = fsm,
						ServerType = ServerType.Document
					});
			}
			catch (Exception Ex)
			{
				//ExceptionLogManager.Instance.LogError(Ex.Message + Ex.StackTrace, Convert.ToString(this.GetType().Name));
				logger.LogError($"Error getting file servers", this.GetType().Name);
			}

			return new List<ResourceServer>();
		}

		/// <summary>
		/// Gets a list of Resource Servers with their IP Addresses.
		/// </summary>
		public IList<ResourceServer> GetAllServersWithIps(IList<ResourceServer> servers, ILogger logger)
		{
			//region Create IP based server list
			return servers
				.SelectMany(server => GetHostAddress(GetServerNameForIpAddress(server), logger).Select(ip => new { server, ServerIp = ip }))
				.Select(s =>
					new ResourceServer()
					{
						ArtifactID = s.server.ArtifactID,
						Name = s.server.Name,
						IP = s.ServerIp.ToString(),
						ServerInstance = s.server.ServerInstance,
						ServerType = s.server.ServerType
					})
				.ToList();
		}

		/// <summary>
		/// Parses out server name from a resource server. Uses the Resource server Name unless it's a database server with a port number
		/// </summary>
		public static string GetServerNameForIpAddress(ResourceServer server)
		{
			var result = server.Name;
			// if it's a DB server than anything after the last comma (,) should be the port number
			var lastCommaIndex = result.LastIndexOf(',');
			result = (lastCommaIndex > 0 && server.ServerType == ServerType.Database) ? result.Substring(0, lastCommaIndex) : result;
			if (server.ServerType == ServerType.Database && result.Contains('\\'))
				result = result.Split('\\')[0];
			return result;
		}

		/// <summary>
		/// Gets the IP Addresses for a given server name
		/// </summary>
		internal static IEnumerable<IPAddress> GetHostAddress(string serverName, ILogger logger)
		{
			try
			{
				return Dns.GetHostAddresses(serverName).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			}
			catch (SocketException)
			{
				//ignore servers that cause a "No such host is known" SocketException
				logger.LogVerbose($"Error getting host IP Address for {serverName}", typeof(ResourceServerRepository).Name);
			}
			return new IPAddress[0];
		}

		public void MergeServerInformation(XElement xml)
		{
			//eddsdbo.MergeServerInformation
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("eddsperformance.eddsdbo.MergeServerInformation", new {XMLServerList = xml.ToString()},
					commandType: CommandType.StoredProcedure, commandTimeout: Defaults.Database.ConnectionTimeout);
			}
		}

		public ServerType GetResourceServerType(string codeName)
		{
			switch (codeName.ToLower())
			{
				case "agent":
					return ServerType.Agent;
				case "services":
					return ServerType.Services;
				case "sql - primary":
				case "sql - distributed":
				case "sql - replica":
					return ServerType.Database;
				case "web":
				case "web:forms authentication":
				case "web:ad authentication":
				case "web - distributed":
				case "web - distributed:forms authentication":
				case "web - distributed:ad authentication":
					return ServerType.Web;
				case "webapi":
				case "webapi:forms authentication":
				case "webapi:ad authentication":
					return ServerType.WebApi;
				case "web background processing":
					return ServerType.WebBackground;
				case "processing server":
					return ServerType.Processing;
				case "analytics server":
					return ServerType.Analytics;
				case "worker manager server":
					return ServerType.Invariant;
				case "worker":
					return ServerType.InvariantWorker;
				case "cache location server":
					return ServerType.CacheLocation;
				default:
					return ServerType.Unrecognized;
			}

		}
	}
}