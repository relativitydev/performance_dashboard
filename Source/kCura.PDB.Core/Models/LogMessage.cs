namespace kCura.PDB.Core.Models
{
	using System;

	[Serializable]
	public class LogMessage
	{
		public LogMessage(LogSeverity severity, string message)
		{
			this.Severity = severity;
			this.Message = message;
		}

		public LogSeverity Severity { get; set; }

		public string Message { get; set; }

		public override string ToString()
		{
			return $"{this.Severity.ToString().ToUpper()}: {this.Message}";
		}
	}
}