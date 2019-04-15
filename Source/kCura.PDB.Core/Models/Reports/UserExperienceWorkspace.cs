namespace kCura.PDB.Core.Models.Reports
{
	using kCura.PDB.Core.Constants;

	public class UserExperienceWorkspace
	{
		public int HourId { get; set; }

		public int WorkspaceId { get; set; }

		public int ServerId { get; set; }

		public int SearchId { get; set; }

		public string SearchName { get; set; }

		public bool IsComplex { get; set; }

		public long TotalExecutionTime { get; set; }

		public int TotalSearchAudits { get; set; }

		public long AverageRunTime
		{
			get { return this.TotalExecutionTime / this.TotalSearchAudits; }
		}
	}
}