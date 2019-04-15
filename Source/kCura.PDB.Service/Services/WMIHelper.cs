namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Management;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class WMIHelper : IWMIHelper
	{
		public WMIHelper(ILogger logger)
		{
			_logger = logger;
		}

		private ILogger _logger;

		public List<KeyValuePair<string, string>> CreateDiagnostics(Server server, String wmiQuery, String category)
		{
			var objectQuery = new ObjectQuery(wmiQuery);
			return ExecuteSearch(server, objectQuery, category);
		}

		/// <summary>
		/// Create diagnostic result
		/// </summary>
		/// <param name="server">Server from which to obtain diagnostic information</param>
		/// <param name="fields">Parameters for fields</param>
		/// <param name="table">Parameter for table name</param>
		/// <param name="where">Where condition</param>
		/// <returns>Return collection of diagnostic result</returns>
		public List<KeyValuePair<string, string>> CreateDiagnostics(Server server, ManagementField fields, String table, String where, String category)
		{
			ObjectQuery objectQuery = null;
			if (fields == ManagementField.All)
			{
				objectQuery = new ObjectQuery($"select * from {table} {@where}");
			}
			else
			{
				objectQuery = new ObjectQuery($"select {fields} from {table} {@where}");
			}

			return ExecuteSearch(server, objectQuery, category);
		}

		/// <summary>
		/// Create diagnostic result
		/// </summary>
		/// <param name="server">Server from which to obtain diagnostic information</param>
		/// <param name="fields">Parameters for fields</param>
		/// <param name="table">Parameter for table name</param>
		/// <returns>Return collection of diagnostic result</returns>
		public List<KeyValuePair<string, string>> CreateDiagnostics(Server server, ManagementField fields, String table, String category)
		{
			ObjectQuery objectQuery = null;
			if (fields == ManagementField.All)
			{
				objectQuery = new ObjectQuery($"select * from {table}");
			}
			else
			{
				objectQuery = new ObjectQuery($"select {fields} from {table}");
			}

			return ExecuteSearch(server, objectQuery, category);
		}

		private List<KeyValuePair<string, string>> ExecuteSearch(Server server, ObjectQuery objectQuery, String category)
		{
			var list = new List<KeyValuePair<string, string>>();

			try
			{
				_logger.LogVerbose($"ExecuteSearch Called for IP '{server.ServerIpAddress}'. Query: {objectQuery.QueryString}", category);

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

				_logger.LogVerbose(
					$"ExecuteSearch Called for IP '{server.ServerIpAddress}'. Query: {objectQuery.QueryString} - Success", category);

			}
			catch (Exception ex)
			{
				_logger.LogError(
					$"ExecuteSearch Called for IP '{server.ServerIpAddress}'. Query: {objectQuery.QueryString} - Failure. Server: {server.ServerName}. Details: {ex.Message}", ex, category);
				throw ex;
			}

			return list;
		}

		private static ManagementScope GetManagementScope(string ipaddress)
		{
			ConnectionOptions connectionOptions = new ConnectionOptions();
			connectionOptions.Authentication = AuthenticationLevel.PacketPrivacy;
			connectionOptions.Impersonation = System.Management.ImpersonationLevel.Impersonate;

			ManagementScope _managementScope = new ManagementScope($"\\\\{ipaddress}\\root\\CIMV2", connectionOptions);
			_managementScope.Connect();

			return _managementScope;
		}
	}
}
