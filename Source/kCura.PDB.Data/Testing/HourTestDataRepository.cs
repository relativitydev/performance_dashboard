namespace kCura.PDB.Data.Testing
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Dapper;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Repositories;

	public class HourTestDataRepository : IHourTestDataRepository
	{
		private readonly IConnectionFactory connectionFactory;
		private readonly HourRepository concreteHourRepository;

		public HourTestDataRepository(IConnectionFactory connectionFactory, HourRepository hourRepository)
		{
			this.connectionFactory = connectionFactory;
			this.concreteHourRepository = hourRepository;
		}

		public async Task CreateAsync(IList<MockHour> mockData)
		{
			// Create data in Hour table, then store hourIds in MockHours table
			// ... reads should do a join on that table

			if (mockData.Any() == false)
			{
				return;
			}

			var hours = mockData.Select(m => new Hour { HourTimeStamp = m.HourTimeStamp, Status = HourStatus.Pending }).ToList();
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				var createdHours = this.concreteHourRepository.Create(hours);
				await conn.ExecuteAsync(Resources.MockHours_Create, new { id = createdHours.Select(h => h.Id).ToList() });
			}
		}

		public async Task ClearAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				await conn.ExecuteAsync(Resources.MockHours_Clear);
			}
		}

		public async Task<IList<Hour>> ReadHoursAsync()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return (await conn.QueryAsync<Hour>(Resources.MockHours_Read)).ToList();
			}
		}
	}
}
