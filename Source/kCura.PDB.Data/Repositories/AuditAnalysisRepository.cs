using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using kCura.PDB.Core.Interfaces.Repositories;
using kCura.PDB.Core.Interfaces.Services;
using kCura.PDB.Core.Models;
using kCura.PDB.Core.Models.MetricDataSources;
using kCura.PDB.Data.Properties;

namespace kCura.PDB.Data.Repositories
{
	public class AuditAnalysisRepository : IAuditAnalysisRepository
	{
		public AuditAnalysisRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		private readonly IConnectionFactory connectionFactory;

		public async Task CreateAsync(IList<AuditAnalysis> auditAnalyses)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				foreach (var auditAnalysis in auditAnalyses)
				{
					var existingAudtAnalysis = await conn.QueryFirstOrDefaultAsync<AuditAnalysis>(Resources.AuditAnalysis_ReadByMetricDataAndUser, auditAnalysis);
					if(existingAudtAnalysis != null)
						await conn.ExecuteAsync(Resources.AuditAnalysis_Create, auditAnalysis);
				}
			}
		}

		public async Task<IList<AuditAnalysis>> ReadByMetricData(MetricData metricData)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<AuditAnalysis>(Resources.AuditAnalysis_ReadByMetricData, new { metricDataId = metricData.Id })).ToList();
			}
		}
	}
}
