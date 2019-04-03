namespace kCura.PDB.Service.Servers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Servers;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public class DatabaseService : IDatabaseService
	{
		//private readonly IResourceServerRepository resourceServerRepository;
		private readonly IServerRepository serverRepository;
		private readonly IDatabaseRepository databaseRepository;
		private readonly ITimeService timeService;

		public DatabaseService(IServerRepository serverRepository, IDatabaseRepository databaseRepository, ITimeService timeService)
		{
			this.serverRepository = serverRepository;
			this.databaseRepository = databaseRepository;
			this.timeService = timeService;
		}

		public async Task UpdateTrackedDatabasesAsync()
		{
			var allServers = await this.serverRepository.ReadAllActiveAsync();
			await allServers.Where(s => s.ServerType == ServerType.Database)
				.Select(UpdateTrackedDatabaseAsync)
				.WhenAllStreamed(1);
		}

		/// <summary> 
		/// Returns gap models as if they were created, but are not stored in the data store 
		/// </summary> 
		/// <param name="hour">Current hour</param> 
		/// <param name="server">Server to get databases from</param> 
		/// <returns></returns> 
		public async Task<IList<Gap>> ReadUnresolvedGapsAsync(Hour hour, Server server, GapActivityType activityType)
		{
			var databases = await this.databaseRepository.ReadByServerIdAsync(server.ServerId);
			switch (activityType)
			{
				case GapActivityType.Backup:
					return databases.Where(db => db.MostRecentBackupAnyType.HasValue && db.MostRecentBackupAnyType.Value < hour.HourTimeStamp)
						.Select(
							db => new Gap
							{
								ActivityType = activityType,
								DatabaseId = db.Id,
								Start = db.MostRecentBackupAnyType.Value,
								Duration = (int)(hour.HourTimeStamp - db.MostRecentBackupAnyType.Value).TotalSeconds
							}).ToList();
				case GapActivityType.BackupFullAndDiff:
					return databases.Where(db => (db.LastBackupDiffDate.HasValue && db.LastBackupDiffDate.Value < hour.HourTimeStamp) || (db.LastBackupFullDate.HasValue && db.LastBackupFullDate.Value < hour.HourTimeStamp))
						.Select(
							db =>
							{
								var start = (db.LastBackupDiffDate.HasValue && db.LastBackupDiffDate.Value < hour.HourTimeStamp)
												? db.LastBackupDiffDate.Value
												: db.LastBackupFullDate.Value;
								return new Gap
								{
									ActivityType = activityType,
									DatabaseId = db.Id,
									Start = start,
									Duration = (int)(hour.HourTimeStamp - start).TotalSeconds
								};
							}).ToList();
				case GapActivityType.Dbcc:
					return databases.Where(db => db.LastDbccDate.HasValue && db.LastDbccDate.Value < hour.HourTimeStamp)
						.Select(
							db => new Gap
							{
								ActivityType = activityType,
								DatabaseId = db.Id,
								Start = db.LastDbccDate.Value,
								Duration = (int)(hour.HourTimeStamp - db.LastDbccDate.Value).TotalSeconds
							}).ToList();
				default:
					throw new ArgumentOutOfRangeException(nameof(activityType), activityType, null);
			}
		}

		internal async Task UpdateTrackedDatabaseAsync(Server server)
		{
			// previous mean the list we have in our database that may not be up-to-date
			var previousDatabases = this.databaseRepository.ReadByServerIdAsync(server.ServerId);

			// Current means just freshly read from the database
			var currentServerDatabaseNames = this.databaseRepository.GetByServerAsync(server);

			await Task.WhenAll(previousDatabases, currentServerDatabaseNames);

			await this.MarkDatabasesDeleted(await currentServerDatabaseNames, await previousDatabases);
			await this.CreateNewDatabases(server, await currentServerDatabaseNames, await previousDatabases);
		}

		/// <summary>
		/// Marks databases that are not found on the server anymore
		/// </summary>
		/// <param name="currentServerDatabaseNames">Current means just freshly read from the database</param>
		/// <param name="previousServerDatabases">previous means the list we have in our database that may not be up-to-date</param>
		/// <returns>Task</returns>
		internal async Task MarkDatabasesDeleted(IList<string> currentServerDatabaseNames, IList<Database> previousServerDatabases) =>
			await previousServerDatabases
				.Where(psd => currentServerDatabaseNames.All(csdn => csdn != psd.Name) || (psd.Type != DatabaseType.Primary && psd.Type != DatabaseType.Workspace)) // find previous databases that are not in the current database list
				.ForEach(psd => psd.DeletedOn = this.timeService.GetUtcNow()) // mark those databases deleted
				.Select(psd => this.databaseRepository.UpdateAsync(psd)) // update the database record
				.WhenAllStreamed(4);

		/// <summary>
		/// Creates new databases
		/// </summary>
		/// <param name="server">The server</param>
		/// <param name="currentServerDatabaseNames">Current means just freshly read from the database</param>
		/// <param name="previousServerDatabases">Previous means the list we have in our database that may not be up-to-date</param>
		/// <returns>Task</returns>
		internal async Task CreateNewDatabases(Server server, IList<string> currentServerDatabaseNames, IList<Database> previousServerDatabases) =>
			await currentServerDatabaseNames
				.Where(csdn => previousServerDatabases.All(psd => csdn != psd.Name)) // find databases names that are not in the previous database list
				.Select(csdn => new Database { Name = csdn, ServerId = server.ServerId, Type = GetDatabaseType(csdn), WorkspaceId = GetDatabaseWorkspace(csdn) })
				.Where(csd => csd.Type == DatabaseType.Primary || csd.Type == DatabaseType.Workspace) // Only grab Primary and Workspace databases
				.Select(csd => this.databaseRepository.CreateAsync(csd)) // create the database record
				.WhenAllStreamed(4);

		public static int? GetDatabaseWorkspace(string databaseName)
		{
			if (databaseName == Names.Database.Edds)
				return null;

			var eddsWorkspace = Regex.Matches(databaseName, @"edds(\d+)$", RegexOptions.IgnoreCase).GetCaptures().ToList();
			if (eddsWorkspace.Any())
				return Convert.ToInt32(eddsWorkspace.First());

			var invariantWorkspace = Regex.Matches(databaseName, @"inv(\d+)$", RegexOptions.IgnoreCase).GetCaptures().ToList();
			if (invariantWorkspace.Any())
				return Convert.ToInt32(invariantWorkspace.First());

			return null;
		}

		public static DatabaseType GetDatabaseType(string databaseName)
		{
			if (databaseName.ToLower() == Names.Database.Edds.ToLower())
				return DatabaseType.Primary;

			var eddsWorkspace = Regex.Matches(databaseName, @"edds(\d+)$", RegexOptions.IgnoreCase).GetCaptures().ToList();
			if (eddsWorkspace.Any())
				return DatabaseType.Workspace;

			var invariantWorkspace = Regex.Matches(databaseName, @"inv(\d+)$", RegexOptions.IgnoreCase).GetCaptures().ToList();
			if (invariantWorkspace.Any())
				return DatabaseType.Invariant;

			return DatabaseType.Other;
		}
	}
}
