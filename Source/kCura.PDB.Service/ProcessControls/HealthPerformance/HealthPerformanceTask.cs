namespace kCura.PDB.Service.ProcessControls.HealthPerformance
{
	using System;
	using System.Collections.Generic;
	using System.Management;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public abstract class HealthPerformanceTask
	{
		public ISqlServerRepository SqlService { get; set; }
		public ILogger Logger { get; set; }

		#region properties

		internal protected ManagementScope GetManagementScope(string ipaddress)
		{
			ConnectionOptions connectionOptions = new ConnectionOptions();
			connectionOptions.Authentication = AuthenticationLevel.PacketPrivacy;
			connectionOptions.Impersonation = ImpersonationLevel.Impersonate;

			ManagementScope _managementScope = new ManagementScope(string.Format("\\\\{0}\\root\\CIMV2", ipaddress), connectionOptions);
			_managementScope.Connect();

			return _managementScope;
		}

		#endregion

		/// <summary>
		/// Inserting entity
		/// </summary>
		protected List<BaseDW> ServerList { get; set; }

		public HealthPerformanceTask()
		{
			ServerList = new List<BaseDW>();
		}

		internal abstract void ProcessServer(Server server);

		internal abstract void SavePerformanceMetrics();

		internal protected List<KeyValuePair<string, string>> CreateDiagnostics(Server server, string wmiQuery)
		{
			var objectQuery = new ObjectQuery(wmiQuery);
			return ExecuteSearch(server, objectQuery);
		}

		/// <summary>
		/// Create diagnostic result
		/// </summary>
		/// <param name="server">Server from which to obtain diagnostic information</param>
		/// <param name="fields">Parameters for fields</param>
		/// <param name="table">Parameter for table name</param>
		/// <param name="where">Where condition</param>
		/// <returns>Return collection of diagnostic result</returns>
		internal protected List<KeyValuePair<string, string>> CreateDiagnostics(Server server, ManagementField fields, string table, string where)
		{
			ObjectQuery objectQuery = null;
			if (fields == ManagementField.All)
			{
				objectQuery = new ObjectQuery(string.Format("select {0} from {1} {2}", "*", table, where));
			}
			else
			{
				objectQuery = new ObjectQuery(string.Format("select {0} from {1} {2}", fields, table, where));
			}

			return ExecuteSearch(server, objectQuery);
		}

		private List<KeyValuePair<string, string>> ExecuteSearch(Server server, ObjectQuery objectQuery)
		{
			var list = new List<KeyValuePair<string, string>>();

			try
			{
				Logger.LogVerbose(string.Format("ExecuteSearch Called for IP '{0}'. Query: {1}", server.ServerIpAddress, objectQuery.QueryString), GetType().Name);

				using (var searcher = new ManagementObjectSearcher(GetManagementScope(server.ServerIpAddress), objectQuery))
				{
					foreach (ManagementObject managementObject in searcher.Get())
					{
						foreach (var prop in managementObject.Properties)
						{
							list.Add(new KeyValuePair<string, string>(prop.Name, (prop.Value ?? string.Empty).ToString()));
						}
					}
				}

				Logger.LogVerbose(string.Format("ExecuteSearch Called for IP '{0}'. Query: {1} - Success", server.ServerIpAddress, objectQuery.QueryString), GetType().Name);

			}
			catch (Exception ex)
			{
				Logger.LogError(string.Format("ExecuteSearch Called for IP '{0}'. Query: {1} - Failure. Server: {2}. Details: {3}",
										server.ServerIpAddress,
										objectQuery.QueryString,
										server.ServerName,
										ex.Message), GetType().Name);
				throw ex;
			}

			return list;
		}
	}

}
