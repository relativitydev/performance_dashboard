namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Threading.Tasks;

	public interface ICleanupTablesRepository
	{
		/// <summary>
		/// Dynamic delete of a table in EddsPerformance
		/// </summary>
		/// <param name="tableScope">The table scope string that denotes the table to delete from (eg. "eddsdbo.QoS_SourceDatetime")</param>
		/// <param name="dateTimeColumn">The column name of a datetime column on the given table</param>
		/// <param name="threshold">The datetime threshold which to delete from (deletes everything BEFORE this given datetime)</param>
		/// <param name="batchSize">Max records to delete in a single delete call</param>
		/// <param name="maxdopLimit">Flag to add (MAXDOP 2) option to the dynamically generated sql</param>
		/// <returns>Task to await on</returns>
		Task CleanupPerformanceTable(string tableScope, string dateTimeColumn, DateTime threshold, int batchSize, bool maxdopLimit = true);

		/// <summary>
		/// Delete of QoS_GlassRunLog and EventLogs table
		/// </summary>
		/// <param name="threshold">The datetime threshold which to delete from (deletes everything BEFORE this given datetime)</param>
		/// <param name="batchSize">Max records to delete in a single delete call</param>
		/// <param name="maxdopLimit">Flag to add (MAXDOP 2) option to the dynamically generated sql</param>
		/// <returns>Task to await on</returns>
		Task CleanupQoSGlassRunLog(DateTime threshold, int batchSize, bool maxdopLimit = true);
		
		/// <summary>
		/// Dynamic delete of a table in EddsQoS
		/// </summary>
		/// <param name="serverName">Name of the server to connect to and perform deletes</param>
		/// <param name="tableScope">The table scope string that denotes the table to delete from (eg. "eddsdbo.QoS_VarscatOutput")</param>
		/// <param name="threshold">The qosHourId threshold which to delete from (deletes everything less than this given threshold)</param>
		/// <param name="batchSize">Max records to delete in a single delete call</param>
		/// <param name="maxdopLimit">Flag to add (MAXDOP 2) option to the dynamically generated sql</param>
		/// <returns>Task to await on</returns>
		Task CleanupQosTable(string serverName, string tableScope, long threshold, int batchSize, bool maxdopLimit = true);

		Task CleanupQosTable(
			string serverName,
			string tableScope,
			string dateTimeColumn,
			DateTime threshold,
			int batchSize,
			bool maxdopLimit = true);

		/// <summary>
		/// Cleanup method to delete all decommissioned servers from CasesToAudit table
		/// </summary>
		/// <param name="threshold">The datetime threshold which to delete from (deletes everything AFTER and EQUAL TO this given datetime)</param>
		/// <param name="batchSize">Max records to delete in a single delete call</param>
		/// <param name="maxdopLimit">Flag to add (MAXDOP 2) option to the dynamically generated sql</param>
		/// <returns>Task to await on</returns>
		Task CleanupDecommissionedServers(DateTime threshold, int batchSize, bool maxdopLimit = true);
	}
}
