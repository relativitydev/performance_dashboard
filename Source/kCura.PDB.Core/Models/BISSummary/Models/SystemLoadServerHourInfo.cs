namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class SystemLoadServerHourInfo
	{
		public int Index;
		public int ServerId;
		public string Server;
		public string ServerType;
		public DateTime SummaryDayHour;
		public int OverallScore;
		public int CPUScore;
		public int RAMScore;
        public int MemorySignalStateScore;
        public string FriendlyMemorySignalStateScore => ServerType == "SQL"
	        ? MemorySignalStateScore.ToString()
	        : "N/A";

		public int WaitsScore;
        public string FriendlyWaitsScore => ServerType == "SQL"
	        ? WaitsScore.ToString()
	        : "N/A";

		public int VirtualLogFilesScore;
        public string FriendlyVirtualLogFilesScore => ServerType == "SQL"
	        ? VirtualLogFilesScore.ToString()
	        : "N/A";

		public int LatencyScore;
        public string FriendlyLatencyScore => ServerType == "SQL"
	        ? LatencyScore.ToString()
	        : "N/A";

		public bool IsActiveWeeklySample;

        public int MemorySignalStateRatio;
        public int Pageouts;
        public int MaxVirtualLogFiles;
        public string HighestLatencyDatabase;
        public int ReadLatencyMs;
        public int WriteLatencyMs;
        public bool IsDataFile;
        public string DatabaseFileType => IsDataFile
	        ? "Data"
	        : "Log";
	}
}
