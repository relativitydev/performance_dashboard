namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class SystemLoadForecast
	{
		public int ServerTypeId;
		public string ServerTypeName => ServerTypeId == 3 ? "SQL" : "Web";
		public string ServerName;
		public int CPUScore;
		public int RAMScore;
        public int SQLMemoryScore;
        public int SQLWaitsScore;
        public int SQLVirtualLogFilesScore;
	}
}
