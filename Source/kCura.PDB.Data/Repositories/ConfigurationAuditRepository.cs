namespace kCura.PDB.Data.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class ConfigurationAuditRepository : BaseDbRepository, IConfigurationAuditRepository
	{
		public ConfigurationAuditRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
			
		}

		public IList<ConfigurationAudit> ReadAll()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return conn.Query<ConfigurationAudit>(Resources.ConfigurationAudit_ReadAll).AsList();
			}
		}

		public void Create(IList<ConfigurationAudit> audits)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				foreach(var audit in audits)
				{
					conn.Execute(Resources.ConfigurationAudit_Create, new { audit.FieldName, audit.ServerName, audit.OldValue, audit.NewValue, audit.UserId, audit.CreatedOn });
				}
			}
			
		}
	}
}
