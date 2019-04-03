namespace kCura.PDB.Core.Models.Reports
{
	public class UserExperienceSearch
	{
		public int HourId { get; set; }

		public int SearchId { get; set; }

		public int MinAuditId { get; set; }

		public int UserId { get; set; }

		public int WorkspaceId { get; set; }

		public int ServerId { get; set; }

		public decimal PercentLongRunning { get; set; }

		public bool? IsComplex { get; set; }

		/// <summary>
		/// Gets or sets TotalExecutionTime (aka TotalRunTime)
		/// </summary>
		public int TotalExecutionTime { get; set; }

		/// <summary>
		/// Gets or sets TotalSearchAudits (aka TotalRuns)
		/// </summary>
		public int TotalSearchAudits { get; set; }

		public long AverageRunTime
		{
			get { return this.TotalExecutionTime / this.TotalSearchAudits; }
		}
	}
}
