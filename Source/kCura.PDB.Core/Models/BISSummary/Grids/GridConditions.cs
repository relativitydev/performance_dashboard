namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System;

	public class GridConditions
	{
		public GridConditions()
		{
			StartRow = 1;
			EndRow = 25;
		}

		public string sEcho;
		public string SortIndex;
		public string SortColumn;
		public string SortDirection;

		public int StartRow;
		public int EndRow;

		public int TimezoneOffset;
		public DateTime? StartDate;
		public DateTime? EndDate;
		public string SelectedServers;
		public int ServerArtifactId;
	}
}
