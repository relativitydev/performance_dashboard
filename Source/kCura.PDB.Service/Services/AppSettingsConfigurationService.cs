namespace kCura.PDB.Service.Services
{
	using System;
	using System.Configuration;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Services;

	public class AppSettingsConfigurationService : IAppSettingsConfigurationService
	{
		public bool ContainsAppSettingsKey(string key)
		{
			return ConfigurationManager.AppSettings.AllKeys.Any(k => k == key);
		}

		public bool ContainsAppSettingsKeys(params string[] keys)
		{
			return ConfigurationManager.AppSettings.AllKeys.Count(k => keys.Any(ks => ks == k)) == keys.Length;
		}

		public string GetAppSetting(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}

		public ConnectionStringSettings GetConnectionStrings(string name)
		{
			return ConfigurationManager.ConnectionStrings[name];
		}

		public SqlConnectionStringBuilder GetConnectionStringBuilder(string connectionString, bool errorIfNotFound = true)
		{
			var connStr = this.GetConnectionStrings(connectionString)?.ConnectionString;
			if (string.IsNullOrEmpty(connStr) && errorIfNotFound)
			{
				throw new Exception($"There is no connection string or connection information configured for {connectionString}.");
			}

			return string.IsNullOrEmpty(connStr) ? null : new SqlConnectionStringBuilder(connStr);
		}
	}
}
