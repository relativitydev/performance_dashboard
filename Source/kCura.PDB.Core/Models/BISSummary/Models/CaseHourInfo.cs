namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class CaseHourInfo
	{
		public int CaseHourId { get; set; }
		public int ServerArtifactId { get; set; }
		public int CaseArtifactId { get; set; }
		public DateTime? SummaryDayHour { get; set; }
	}
}
