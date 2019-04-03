namespace kCura.PDB.Service.Hours
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class HourCleanupLogic : IHourCleanupLogic
	{
		private readonly IHourRepository hourRepository;
		private readonly IServerRepository serverRepository;
		private readonly ICleanupTablesRepository cleanupTablesRepository;
		private readonly IDataIntegrityRepository dataIntegrityRepository;
		private readonly IServerCleanupRepository serverCleanupRepository;
		private readonly ILogger logger;

		public HourCleanupLogic(
			IHourRepository hourRepository,
			IServerRepository serverRepository,
			ICleanupTablesRepository cleanupTablesRepository,
			IDataIntegrityRepository dataIntegrityRepository,
			IServerCleanupRepository serverCleanupRepository,
			ILogger logger)
		{
			this.hourRepository = hourRepository;
			this.serverRepository = serverRepository;
			this.cleanupTablesRepository = cleanupTablesRepository;
			this.dataIntegrityRepository = dataIntegrityRepository;
			this.serverCleanupRepository = serverCleanupRepository;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Hour);
		}

		public async Task<IList<int>> CleanupForHour(int hourId)
		{
			await this.logger.LogVerboseAsync($"Starting database cleanup");

			// In this logic, we need to cleanup various tables based on the given hourId
			// Cleanup performance tables
			await this.CleanupPerformanceTables(hourId);

			// Get server ids for queuing qos tables cleanup
			var servers = (await this.serverRepository.ReadAllActiveAsync())
				.Where(s => s.ServerType == ServerType.Database);
			var serverCleanups =
				await servers
				.Select(s => this.serverCleanupRepository.CreateAsync(new ServerCleanup { HourId = hourId, ServerId = s.ServerId }))
				.WhenAllStreamed();
			return serverCleanups.Select(sc => sc.Id).ToList();
		}

		public async Task<int> CleanupQosTables(int serverCleanupId)
		{
			var serverCleanup = await this.serverCleanupRepository.ReadAsync(serverCleanupId);
			var hour = await this.hourRepository.ReadAsync(serverCleanup.HourId);
			var server = await this.serverRepository.ReadAsync(serverCleanup.ServerId);
			try
			{
				await this.logger.LogVerboseAsync($"CleanupForHour Called on {server.ServerId} - {server.ServerName} EddsQoS for hour: {hour.Id} - {hour.HourTimeStamp}");

				var dropTriggersTask = this.dataIntegrityRepository.DropAllTriggersInCurrentDatabase(server);

				var varscatTask = this.cleanupTablesRepository.CleanupQosTable(
					server.ServerName,
					$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputTable}",
					Names.Database.SummaryDayHourColumn,
					hour.HourTimeStamp.AddDays(DatabaseConstants.PastWeekThreshold),
					Defaults.Database.DeleteBatchSize);

				var varscatDetailTask = this.cleanupTablesRepository.CleanupQosTable(
					server.ServerName,
					$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputDetailTable}",
					Names.Database.TimestampColumn,
					hour.HourTimeStamp.AddDays(DatabaseConstants.PastWeekThreshold),
					Defaults.Database.DeleteBatchSize);

				await Task.WhenAll(dropTriggersTask, varscatTask, varscatDetailTask);

				serverCleanup.Success = true;
				await this.serverCleanupRepository.UpdateAsync(serverCleanup);
			}
			catch (Exception ex)
			{
				this.logger.LogError(
					server != null
						? $"Failed to cleanup qos database tables. Server: {server.ServerName} for server cleanup: {serverCleanupId}"
						: $"Failed to cleanup qos database tables. Server cleanup: {serverCleanupId}", ex);
			}

			return hour.Id;
		}

		internal async Task CleanupPerformanceTables(int hourId)
		{
			var hour = await this.hourRepository.ReadAsync(hourId);

			await this.cleanupTablesRepository.CleanupQoSGlassRunLog(
				hour.HourTimeStamp.AddDays(DatabaseConstants.GlassRunLogDeleteThresholdDays),
				Defaults.Database.DeleteBatchSize);

			await this.cleanupTablesRepository.CleanupPerformanceTable(
				$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputCumulativeTable}",
				Names.Database.SummaryDayHourColumn,
				hour.HourTimeStamp.AddDays(DatabaseConstants.PastQuarterThreshold),
				Defaults.Database.DeleteBatchSize);

			await this.cleanupTablesRepository.CleanupPerformanceTable(
				$"{Names.Database.EddsdboSchema}.{Names.Database.VarscatOutputDetailCumulativeTable}",
				Names.Database.SummaryDayHourColumn,
				hour.HourTimeStamp.AddDays(DatabaseConstants.PastQuarterThreshold),
				Defaults.Database.DeleteBatchSize);

			await this.cleanupTablesRepository.CleanupDecommissionedServers(
				hour.HourTimeStamp,
				Defaults.Database.DeleteBatchSize);

			await this.dataIntegrityRepository.DropAllTriggersInCurrentDatabase();
		}
	}
}
