namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class LookingGlassRunInfo
	{
		public DateTime? RunTime { get; set; }
		public int FailedCases { get; set; }
		public bool IsActive
		{
			get
			{
				return RunTime.HasValue;
			}
		}
	}
}
