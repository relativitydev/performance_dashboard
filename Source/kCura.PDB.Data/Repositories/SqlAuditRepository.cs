namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Data.Properties;

	/// <summary>
	/// Returns the Database audits from SQL Queries
	/// </summary>
	public class SqlAuditRepository : ISqlAuditRepository
	{
		private readonly IConnectionFactory connectionFactory;
		private const int AuditQueryConnectionTimeout = 10 * 60;

		public SqlAuditRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		/// <inheritdoc />
		public async Task<IList<Audit>> ReadAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes, int batchSize, long pageStart)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return (await connection.QueryAsync<Audit>(
					Resources.SqlAudit_ReadAuditsForHour,
					new { beginDate = startHour, endDate = endHour, action = actionTypes, batchSize, pageStart, workspaceId },
					commandTimeout: AuditQueryConnectionTimeout))
					.ToList();
			}
		}

		/// <inheritdoc />
		public async Task<bool> ReadAnyAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return await connection.ExecuteScalarAsync<bool>(Resources.SqlAudit_ReadAnyAudits,
					new { beginDate = startHour, endDate = endHour, action = actionTypes });
			}
		}

		/// <inheritdoc />
		public async Task<long> ReadTotalAuditsForHourAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return await connection.ExecuteScalarAsync<long>(
					Resources.SqlAudit_ReadTotalAuditsForHour,
					new { beginDate = startHour, endDate = endHour, action = actionTypes },
					commandTimeout: AuditQueryConnectionTimeout);
			}
		}

		/// <inheritdoc />
		public async Task<IList<int>> ReadUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return (await connection.QueryAsync<int>(
					Resources.SqlAudit_ReadUniqueUsersForHourAudits,
					new { beginDate = startHour, endDate = endHour, action = actionTypes },
					commandTimeout: AuditQueryConnectionTimeout)).ToList();
			}
		}

		/// <inheritdoc />
		// [NOTE: does not work for Document Query (Search Audits)]
		public async Task<long> ReadTotalLongRunningQueriesForHourAsync(int workspaceId, DateTime startHour, DateTime endHour,
			IList<AuditActionId> actionTypes)
		{
			if (actionTypes.Contains(AuditActionId.DocumentQuery))
			{
				throw new ArgumentException("Method does not support DocumentQuery AuditActionId.", nameof(actionTypes));
			}

			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return await connection.ExecuteScalarAsync<long>(
					Resources.SqlAudit_ReadTotalLongRunningAuditsForHour,
					new { beginDate = startHour, endDate = endHour, action = actionTypes },
					commandTimeout: AuditQueryConnectionTimeout);
			}
		}

		/// <inheritdoc />
		// May not be needed anymore?
		public async Task<long> ReadTotalAuditExecutionTimeForHourAsync(int workspaceId, DateTime startHour, DateTime endHour,
			IList<AuditActionId> actionTypes)
		{
			if (actionTypes.Any(a => AuditConstants.UpdateAuditActionIds.Contains(a)))
			{
				throw new ArgumentException("Method does not support any Update AuditActionIds.  Use Total to determine estimated execution time/concurrency.", nameof(actionTypes));
			}

			using (var connection = await this.connectionFactory.GetWorkspaceConnectionAsync(workspaceId))
			{
				return await connection.ExecuteScalarAsync<long>(
					Resources.SqlAudit_ReadTotalLongRunningAuditsForHour,
					new { beginDate = startHour, endDate = endHour, action = actionTypes },
					commandTimeout: AuditQueryConnectionTimeout);
			}
		}

		public async Task<int> ReadTotalUniqueUsersForHourAuditsAsync(int workspaceId, DateTime startHour, DateTime endHour, IList<AuditActionId> actionTypes)
		{
			return (await this.ReadUniqueUsersForHourAuditsAsync(workspaceId, startHour, endHour, actionTypes)).Count;
		}
	}
}
