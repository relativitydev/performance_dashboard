namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Transactions;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class ConfigurationRepository : BaseDbRepository, IConfigurationRepository
	{
		public ConfigurationRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		/// <inheritdoc />
		public async Task<string> ReadValueAsync(string name)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<string>(Resources.Config_ReadValueBySectionAndName, new { section = ConfigurationKeys.Section, name });
			}
		}

		/// <inheritdoc />
		public async Task<T?> ReadValueAsync<T>(string name) where T : struct
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var value = await conn.QueryFirstOrDefaultAsync<string>(Resources.Config_ReadValueBySectionAndName, new { section = ConfigurationKeys.Section, name });
				if (string.IsNullOrEmpty(value)) return null;
				return value.TryParse<T>();
			}
		}

		/// <inheritdoc />
		public T? ReadValue<T>(string name) where T : struct
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var value = conn.QueryFirstOrDefault<string>(Resources.Config_ReadValueBySectionAndName, new { section = ConfigurationKeys.Section, name });
				if (string.IsNullOrEmpty(value)) return null;
				return value.TryParse<T>();
			}
		}

		public string ReadValue(string name)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.QueryFirstOrDefault<string>(Resources.Config_ReadValueBySectionAndName, new { section = ConfigurationKeys.Section, name });
			}
		}

		/// <inheritdoc />
		public string ReadConfigurationValue(string section, string name)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.QueryFirstOrDefault<string>(Resources.Config_ReadValueBySectionAndName, new { section, name });
			}
		}

		/// <inheritdoc />
		public T ReadConfigurationValue<T>(string section, string name)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.QueryFirstOrDefault<T>(Resources.Config_ReadValueBySectionAndName, new { section, name });
			}
		}

		/// <inheritdoc />
		public void SetConfigurationValue(string section, string name, string val)
		{
			using (var transaction = new TransactionScope())
			{
				using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
				{
					var updatedRecords = conn.Execute(Resources.Config_SetConfigValueUpdate, new { section, name, value = val });
					if (updatedRecords == 0)
					{
						conn.Execute(Resources.Config_SetConfigValueInsert, new { section, name, value = val });
					}
				}
				transaction.Complete();
			}
		}
		
		/// <inheritdoc />
		public async Task SetConfigurationValueEdds(string section, string name, string val)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				await conn.ExecuteAsync(Resources.Config_SetConfigValueUpdate, new { section, name, value = val });
			}
		}

		/// <inheritdoc />
		public IList<RelativityConfigurationInfo> GetRelativityConfigurationInfo()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.Query<RelativityConfigurationInfo>(Resources.Config_ReadInfo).ToArray();
			}
		}

		public async Task<IList<RelativityConfigurationInfo>> ReadEddsConfigurationInfoAsync(string section, string name)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return (await conn.QueryAsync<RelativityConfigurationInfo>(Resources.Config_ReadInfoBySectionAndName,
					new { section, name })).ToList();
			}
		}

		public IList<RelativityConfigurationInfo> ReadEddsConfigurationInfo(string section, string name)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.Query<RelativityConfigurationInfo>(Resources.Config_ReadInfoBySectionAndName,
					new { section, name }).ToList();
			}
		}

		public async Task<IList<RelativityConfigurationInfo>> ReadPdbConfigurationInfoAsync(string section, string name)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<RelativityConfigurationInfo>(Resources.Config_ReadInfoBySectionAndName,
					new { section, name })).ToList();
			}
		}

		public IList<RelativityConfigurationInfo> ReadPdbConfigurationInfo(string section, string name)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.Query<RelativityConfigurationInfo>(Resources.Config_ReadInfoBySectionAndName,
					new { section, name }).ToList();
			}
		}
	}
}
