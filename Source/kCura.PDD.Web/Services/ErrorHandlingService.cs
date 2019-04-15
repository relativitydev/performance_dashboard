namespace kCura.PDD.Web.Services
{
	using System;
	using System.Diagnostics;
	using global::Relativity.CustomPages;
	using global::Relativity.API;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using PDB.Core.Extensions;
	using System.Threading.Tasks;

	public class ErrorHandlingService
	{
		public ErrorHandlingService()
		{

		}

		public ErrorHandlingService(IHelper helper)
		{
			_helper = helper;
		}

		private readonly IHelper _helper;

		public bool LogToErrorLog(Exception ex, String url)
		{
			try
			{
				var helper = _helper ?? ConnectionHelper.Helper();
				var repo = new ErrorRepository(helper);
				repo.LogError(ex, url, -1, Names.Application.PerformanceDashboard);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void LogToErrorTable(Exception ex, string Url)
		{
			try
			{
				var helper = _helper ?? ConnectionHelper.Helper();
				var connectionFactory = new HelperConnectionFactory(helper);
				var repo =
					new LogRepository(connectionFactory);

				Task.Run(() => repo.Create(new LogEntry()
				{
					OtherVars = $"URL: {Url} | Error: {ex.Message} | StackTrace: {ex.StackTrace}",
					Module = "HTTPRequest",
					LogTimestampUtc = DateTime.UtcNow
				})).Wait();
			}
			catch (Exception loggingEx)
			{
				Trace.WriteLine($"Original Error: {ex.ToString()}");
				Trace.WriteLine($"Error while logging: {loggingEx.ToString()}");
			}
		}

		public bool LogToEventViewer(Exception ex, String absoluteUrl, String queryString)
		{
			try
			{
				InitLogger();
				string errorMsg = string.Format("APPLICATION ERROR: {0}\r\nURL :{1} \r\nQUERYSTRING: {2}\r\nSTACKTRACE: {3}.\r\n{4}.\r\n{5}",
							ex.Message.Truncate(500),
							Uri.UnescapeDataString(absoluteUrl),
							queryString,
							ex.ToString(),
							DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt"),
							EventLogEntryType.Error).Truncate(5000);

				EventLog.WriteEntry(Names.Application.PerformanceDashboard, errorMsg, EventLogEntryType.Information, 999);

				return true;
			}
			catch (Exception loggingEx)
			{
				Trace.WriteLine("Original Error: " + ex.ToString());
				Trace.WriteLine("Error while logging: " + loggingEx.ToString());
				return false;
			}
		}

		private static void InitLogger()
		{
			if (!EventLog.SourceExists(Names.Application.PerformanceDashboard))
				EventLog.CreateEventSource(Names.Application.PerformanceDashboard, "Application");
		}

	}
}