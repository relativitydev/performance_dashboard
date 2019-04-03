using System.Threading.Tasks;
using kCura.PDB.Core.Interfaces.Repositories;
using kCura.PDB.Core.Models;
using Dapper;
using kCura.PDB.Core.Helpers;
using kCura.PDB.Core.Interfaces.Services;
using kCura.PDB.Data.Properties;

namespace kCura.PDB.Data.Repositories
{
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;

	public class PoisonWaitRepository : IPoisonWaitRepository
	{
		public PoisonWaitRepository(IConnectionFactory connectionFactory, IProcessControlRepository processControlRepository)
		{
			this.connectionFactory = connectionFactory;
			this.processControlRepository = processControlRepository;
		}

		private readonly IConnectionFactory connectionFactory;
		private readonly IProcessControlRepository processControlRepository;

		public async Task<bool> ReadIfPoisonWaitsForHourAsync(Hour hour)
		{
			ThrowOn.IsNull(hour, "hour cannot be null to check poison waits");
			
			var timeThreshold = hour.GetHourEnd();
			return await this.processControlRepository.HasRunSuccessfully(ProcessControlId.CollectWaitStatistics, timeThreshold);
		}

		public async Task<bool> ReadPoisonWaitsForHourAsync(Hour hour, int serverId)
		{
			ThrowOn.IsNull(hour, "hour cannot be null to check poison waits");
			ThrowOn.IsLessThanOne(serverId, "server must be valid server id to check poison waits");

			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				var result = (await conn.QueryFirstOrDefaultAsync<decimal?>(Resources.PoisonWait_ReadPoisonWaitsForHour,
					new { hour.HourTimeStamp, serverId }));
				return result.HasValue && result.Value > 0;
			}
		}
	}
}
