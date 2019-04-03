namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IConfigurationRepository : IDbRepository
	{
		/// <summary>
		/// Read a value from the EDDSPerformance database's configuration table
		/// </summary>
		/// <param name="name">The configuration's name</param>
		/// <returns>The configuration value</returns>
		Task<string> ReadValueAsync(string name);

		/// <summary>
		/// Read a value from the EDDSPerformance database's configuration table
		/// </summary>
		/// <typeparam name="T">The expected type of the config</typeparam>
		/// <param name="name">The configuration's name</param>
		/// <returns>The configuration value</returns>
		Task<T?> ReadValueAsync<T>(string name)
			where T : struct;

		/// <summary>
		/// Read a value from the EDDSPerformance database's configuration table
		/// </summary>
		/// <typeparam name="T">The expected type of the config</typeparam>
		/// <param name="name">The configuration's name</param>
		/// <returns>The configuration value</returns>
		T? ReadValue<T>(string name)
			where T : struct;

		/// <summary>
		/// Read a value from the EDDSPerformance database's configuration table
		/// </summary>
		/// <param name="name">The configuration's name</param>
		/// <returns>The configuration value</returns>
		string ReadValue(string name);

		/// <summary>
		/// Read a value from the EDDSPerformance database's configuration table
		/// </summary>
		/// <param name="section"></param>
		/// <param name="name"></param>
		/// <returns>String if value is found, null if value is not found</returns>
		string ReadConfigurationValue(string section, string name);

		/// <summary>
		/// Read a value from the EDDSPerformance database's configuration table
		/// </summary>
		/// <param name="section"></param>
		/// <param name="name"></param>
		/// <returns>String if value is found, null if value is not found</returns>
		T ReadConfigurationValue<T>(string section, string name);

		/// <summary>
		/// Set a value in the EDDSPerformance database's configuration table
		/// </summary>
		/// <param name="section"></param>
		/// <param name="name"></param>
		/// <param name="val"></param>
		void SetConfigurationValue(string section, string name, string val);

		/// <summary>
		/// Set a value in the EDDS database's configuration table
		/// </summary>
		/// <param name="section">The configuration section</param>
		/// <param name="name">The configuration name</param>
		/// <param name="val">The configuration value</param>
		/// <returns>Task</returns>
		Task SetConfigurationValueEdds(string section, string name, string val);

		/// <summary>
		/// Set a value in the EDDSPerformance database's configuration table
		/// </summary>
		/// <param name="section"></param>
		/// <param name="name"></param>
		/// <param name="val"></param>
		IList<RelativityConfigurationInfo> GetRelativityConfigurationInfo();

		IList<RelativityConfigurationInfo> ReadEddsConfigurationInfo(string section, string name);

		Task<IList<RelativityConfigurationInfo>> ReadEddsConfigurationInfoAsync(string section, string name);

		IList<RelativityConfigurationInfo> ReadPdbConfigurationInfo(string section, string name);

		Task<IList<RelativityConfigurationInfo>> ReadPdbConfigurationInfoAsync(string section, string name);
	}
}
