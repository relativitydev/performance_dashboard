namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;

	public class ProcessControlRepository : BaseDbRepository, IProcessControlRepository
	{
		public ProcessControlRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<ProcessControl> ReadByIdAsync(ProcessControlId processControlId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<ProcessControl>(Resources.ProcessControl_ReadById,
					new { id = (int)processControlId });
			}
		}

		public ProcessControl ReadById(ProcessControlId processControlId)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return connection.QueryFirstOrDefault<ProcessControl>(Resources.ProcessControl_ReadById,
					new { id = (int)processControlId });
			}
		}

		public async Task<bool> HasRunSuccessfully(ProcessControlId processControlId, DateTime timeThreshold)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await connection.QueryFirstOrDefaultAsync<bool>(Resources.ProcessControl_HasRunSuccessfully,
					new { id = (int)processControlId, timeThreshold });
			}
		}

		public async Task UpdateAsync(ProcessControl processControl)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await connection.ExecuteAsync(Resources.ProcessControl_Update, processControl);
			}
		}

		public void Update(ProcessControl processControl)
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				connection.Execute(Resources.ProcessControl_Update, processControl);
			}
		}

		public async Task<IList<ProcessControl>> ReadAllAsync()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await connection.QueryAsync<ProcessControl>(Resources.ProcessControl_ReadAll)).ToList();
			}
		}

		public IList<ProcessControl> ReadAll()
		{
			using (var connection = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return connection.Query<ProcessControl>(Resources.ProcessControl_ReadAll).ToList();
			}
		}

		/// <summary>
		/// Sets the frequency on a process control
		/// </summary>
		/// <param name="process">ID of the process control to update</param>
		/// <param name="frequency">The new frequency to set</param>
		[Obsolete("Should read the process control and update instead.")]
		public void SetProcessFrequency(ProcessControlId process, int frequency)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.SetProcessControlFrequency, new { frequency, processControlId = (int)process });
			}
		}
	}
}
