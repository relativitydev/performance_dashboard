namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Configuration;
	using System.Data.SqlClient;

	public interface IAppSettingsConfigurationService
	{
		bool ContainsAppSettingsKey(string key);

		bool ContainsAppSettingsKeys(params string[] keys);

		string GetAppSetting(string key);

		ConnectionStringSettings GetConnectionStrings(string name);

		SqlConnectionStringBuilder GetConnectionStringBuilder(string connectionString, bool errorIfNotFound = true);
	}
}
