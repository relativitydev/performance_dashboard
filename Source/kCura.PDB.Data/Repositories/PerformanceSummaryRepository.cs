namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Data;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Dapper;

	public class PerformanceSummaryRepository : BaseDbRepository, IPerformanceSummaryRepository
	{
		public PerformanceSummaryRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}
		
		public void LoadApplicationHealthSummary(DateTime processExecDate)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("[eddsdbo].[LoadApplicationHealthSummary]", new { processExecDate }, commandType: CommandType.StoredProcedure);
			}
		}

		public void LoadErrorHealthDwData(DateTime processExecDate)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("[eddsdbo].[LoadErrorHealthDwData]", new { processExecDate }, commandType: CommandType.StoredProcedure);
			}
		}

		public void LoadUserHealthDwData(DateTime processExecDate)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute("[eddsdbo].[LoadUserHealthDwData]", new { processExecDate }, commandType: CommandType.StoredProcedure);
			}
		}
	}
}
