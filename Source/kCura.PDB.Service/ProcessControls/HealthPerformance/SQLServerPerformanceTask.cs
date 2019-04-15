namespace kCura.PDB.Service.ProcessControls.HealthPerformance
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;

	public class SQLServerPerformanceTask : HealthPerformanceTask
	{
		internal override void ProcessServer(Server server)
		{
			try
			{
				Logger.LogVerbose("ProcessServer Called for SQL", GetType().Name);
				var pageLifeExpectancy = SqlService.GetPageLifeExpectancyFromServerInstance(server);
				var lowMemory = SqlService.GetLowMemorySignalStateFromServerInstance(server);

				ServerList.Add(new SQLServerDW
				{
					CreatedOn = DateTime.UtcNow,
					ServerID = server.ServerId,
					SQLPageLifeExpectancy = pageLifeExpectancy,
					LowMemorySignalState = lowMemory
				});

				Logger.LogVerbose("ProcessServer Called for SQL - Success", GetType().Name);
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				Logger.LogError(string.Format("ProcessServer Called for SQL - Failure. Server: {0}. Details: {1}", server.ServerName, message), GetType().Name);
				throw ex;
			}
		}

		internal override void SavePerformanceMetrics()
		{
			Logger.LogVerbose("SavePerformanceMetrics Called for SQL", GetType().Name);
			using (var dataContext = new PDDModelDataContext())
			{
				try
				{
					if (ServerList.Count > 0)
					{
						dataContext.SQLServerDWs.InsertAllOnSubmit(ServerList.OfType<SQLServerDW>().ToList());
						dataContext.SubmitChanges();
					}

					Logger.LogVerbose(string.Format("SavePerformanceMetrics Called for SQL - Success. Server Count: {0}", ServerList.Count), GetType().Name);
				}
				catch (Exception ex)
				{
					Logger.LogError(string.Format("SavePerformanceMetrics Called for SQL - Failure. Details: {0}", ex.Message), GetType().Name);
					throw ex;
				}
			}
		}
	}
}
