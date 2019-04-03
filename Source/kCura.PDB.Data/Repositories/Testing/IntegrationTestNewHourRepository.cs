namespace kCura.PDB.Data.Repositories.Testing
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Repositories;

	public class IntegrationTestNewHourRepository : IHourRepository
	{
		private readonly IConnectionFactory connectionFactory;
		private readonly HourRepository concreteHourRepository;

		public IntegrationTestNewHourRepository(IConnectionFactory connectionFactory, HourRepository hourRepository)
		{
			this.connectionFactory = connectionFactory;
			this.concreteHourRepository = hourRepository;
		}

		public Task<Hour> CreateAsync(Hour hour)
		{
			throw new System.NotImplementedException();
		}

		public IList<Hour> Create(IList<Hour> hours)
		{
			throw new System.NotImplementedException();
		}

		public Task<Hour> ReadAsync(int id)
		{
			return this.concreteHourRepository.ReadAsync(id);
		}

		public async Task<Hour> ReadHourReadyForScoringAsync(int hourId)
		{
			throw new System.NotImplementedException();
		}

		public Task<Hour> ReadHighestHourAfterMinHour()
		{
			throw new System.NotImplementedException();
		}

		public Task<Hour> ReadLastAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<Hour>> ReadBackFillHoursAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<Hour>> ReadPastWeekHoursAsync(Hour endHour)
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<Hour>> ReadCompletedBackFillHoursAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task UpdateAsync(Hour hour)
		{
			throw new System.NotImplementedException();
		}

		public Task DeleteAsync(Hour hour)
		{
			throw new System.NotImplementedException();
		}

		public Task ScoreHourWithBuildAndRateSampleAsync(Hour hour, decimal weekIntegrityScore, bool enableLogging)
		{
			throw new System.NotImplementedException();
		}
		
		public async Task<Hour> ReadNextHourWithoutRatings()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<Hour>(Resources.HourTest_ReadNextHourWithoutRatings);
			}
		}

		public Task<bool> ReadAnyIncompleteHoursAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<IList<int>> ReadIncompleteHoursAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task<Hour> ReadLatestCompletedHourAsync()
		{
			throw new System.NotImplementedException();
		}
	}
}
