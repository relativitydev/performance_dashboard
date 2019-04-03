namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class UserExperienceWorkspaceHourInfo
	{
		public Int64 Index;
		public int WorkspaceId
		{
			get
			{
				int id;
				int.TryParse((Workspace ?? string.Empty).Replace("EDDS", string.Empty), out id);
				return id;
			}
		}

		public string Workspace;
		public int SearchId;
		public string Search;
		public int TotalRunTime;
		public int AverageRunTime;
		public int TotalRuns;
		public bool IsComplex;
		public DateTime SummaryDayHour;
		public bool IsActiveWeeklySample;
	}
}
