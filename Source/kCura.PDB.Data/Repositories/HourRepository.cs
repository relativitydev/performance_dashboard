namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Transactions;
	using Core.Constants;
	using Core.Extensions;
	using Dapper;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class HourRepository : IHourRepository
	{
		public HourRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		protected readonly IConnectionFactory connectionFactory;

		public async Task<Hour> CreateAsync(Hour hour)
		{
			hour.Status = HourStatus.Pending;
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_Create, hour);
			}
		}

		public IList<Hour> Create(IList<Hour> hours)
		{
			if (hours.Any() == false) return new List<Hour>();
			hours.ForEach(h => h.Status = HourStatus.Pending);
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return hours.Select(hour => conn.QueryFirstOrDefault<Hour>(Resources.Hour_Create, hour)).ToList();
			}
		}

		public async Task<Hour> ReadAsync(int id)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_ReadByID, new { id });
			}
		}

		public async Task<IList<Hour>> ReadAsync(IEnumerable<int> ids)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Hour>(Resources.Hour_ReadByIDs, new { ids })).ToList();
			}
		}

		public async Task<Hour> ReadLastAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_ReadLast);
			}
		}

		public async Task<IList<Hour>> ReadBackFillHoursAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Hour>(Resources.Hour_ReadLastnDays, new { startDate = DateTime.UtcNow.AddDays(Defaults.BackfillDays).NormilizeToHour(), endDate = DateTime.UtcNow.NormilizeToHour() })).ToList();
			}
		}

		// Used for User Experience Sampling
		public async Task<IList<Hour>> ReadPastWeekHoursAsync(Hour endHour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Hour>(Resources.Hour_ReadLastnDays, new { startDate = endHour.HourTimeStamp.AddDays(-7).NormilizeToHour(), endDate = endHour.HourTimeStamp })).ToList();
			}
		}

		public async Task<Hour> ReadCurrentAsync()
		{
			var lastHour = await this.ReadLastAsync();
			return (lastHour != null && DateTime.UtcNow - lastHour.HourTimeStamp < TimeSpan.FromHours(1)) ? lastHour : null;
		}

		public async Task UpdateAsync(Hour hour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Hour_Update, hour);
			}
		}

		public async Task DeleteAsync(Hour hour)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.Hour_Delete, hour);
			}
		}

		public async Task<Hour> ReadHourReadyForScoringAsync(int hourId)
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_ReadHourReadyForScoring, new { hourId });
			}
		}

		/// <inheritdoc />
		public async Task<Hour> ReadHighestHourAfterMinHour()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_ReadHighestHourAfterMinHour);
			}
		}

		public async Task ScoreHourWithBuildAndRateSampleAsync(Hour hour, decimal weekIntegrityScore, bool enableLogging)
		{
			using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var conn = connectionFactory.GetEddsPerformanceConnection())
				{
					await conn.ExecuteAsync("EDDSDBO.QoS_BuildAndRateSample", new { hourId = hour.Id, weekIntegrityScore, logging = enableLogging }, commandType: CommandType.StoredProcedure);
				}
				transaction.Complete();
			}
		}

		public async Task<Hour> ReadNextHourWithoutRatings()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_ReadNextHourWithoutRatings, new { backfillDays = Defaults.BackfillDays });
			}
		}

		public async Task<IList<Hour>> ReadCompletedBackFillHoursAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Hour>(Resources.Hour_ReadCompleteHours, new { backfillDays = Defaults.BackfillDays })).ToList();
			}
		}

		public async Task<bool> ReadAnyIncompleteHoursAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryFirstOrDefaultAsync<int?>(Resources.Hour_ReadIncompleteHours)).HasValue;
			}
		}

		public async Task<IList<int>> ReadIncompleteHoursAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<int>(Resources.Hour_ReadIncompleteHours)).ToList();
			}
		}

		public async Task<Hour> ReadLatestCompletedHourAsync()
		{
			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.Hour_ReadLastCompleted);
			}
		}
	}
}
