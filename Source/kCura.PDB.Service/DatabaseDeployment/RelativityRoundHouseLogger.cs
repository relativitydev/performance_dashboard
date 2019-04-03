namespace kCura.PDB.Service.DatabaseDeployment
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class RelativityRoundHouseLogger : roundhouse.infrastructure.logging.Logger, IRelativityRoundHouseLogger
	{
		public IList<LogMessage> Messages;

		public RelativityRoundHouseLogger()
		{
			Messages = new List<LogMessage>();
		}

		/// <summary> copy this...
		/// new roundhouse.infrastructure.logging.custom.ConsoleLogger();
		/// </summary>
		public object underlying_type { get; set; }

		public void log_a_debug_event_containing(string message, params object[] args)
		{
			Messages.Add(new LogMessage(LogSeverity.Debug, string.Format(message, args)));
			System.Diagnostics.Debug.WriteLine(message, args);
			System.Console.WriteLine(message, args);
		}
		public void log_a_fatal_event_containing(string message, params object[] args)
		{
			Messages.Add(new LogMessage(LogSeverity.Fatal, string.Format(message, args)));
			System.Diagnostics.Debug.WriteLine(message, args);
			System.Console.WriteLine(message, args);
		}

		public void log_a_warning_event_containing(string message, params object[] args)
		{
			Messages.Add(new LogMessage(LogSeverity.Warning, string.Format(message, args)));
			System.Diagnostics.Debug.WriteLine(message, args);
			System.Console.WriteLine(message, args);
		}

		public void log_an_error_event_containing(string message, params object[] args)
		{
			Messages.Add(new LogMessage(LogSeverity.Error, string.Format(message, args)));
			System.Diagnostics.Debug.WriteLine(message, args);
			System.Console.WriteLine(message, args);
		}

		public void log_an_info_event_containing(string message, params object[] args)
		{
			Messages.Add(new LogMessage(LogSeverity.Info, string.Format(message, args)));
			System.Diagnostics.Debug.WriteLine(message, args);
			System.Console.WriteLine(message, args);
		}

	}
}
