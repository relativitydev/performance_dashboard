namespace kCura.PDB.Core.Models
{
	public enum MetricType
	{
		// Uptime
		AgentUptime = 1,
		WebUptime = 2,

		// User Experience
		AuditAnalysis = 12,

		// Infrastructure Performance
		Ram = 20,
		Cpu = 21,
		NumberOfAgentsPerServer = 22,
		SqlServerWaits = 23,
		SqlServerPageOuts = 24,
		SqlServerLatencyForDataFile = 25,
		SqlServerLatencyForLogFile = 26,
		SqlServerVirtualLogFileCount = 27,

		// Recoverability & Integrity
		//Backups = 40,
		//DbccChecks = 41,
		BackupGaps = 42,
		DbccGaps = 43,
		Rpo = 44,
		Rto = 45,
		BackupFrequency = 46,
		DbccFrequency = 47,
		BackupCoverage = 48,
		DbccCoverage = 49,
	}
}
