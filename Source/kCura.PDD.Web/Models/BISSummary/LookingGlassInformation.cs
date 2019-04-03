namespace kCura.PDD.Web.Models.BISSummary
{
	using System;
	using kCura.PDB.Core.Models;

	public class LookingGlassInformation
	{
		public LookingGlassInformation(SampleHistoryRange sampleInfo)
		{
			this.MinSampleDate = sampleInfo.MinSampleDate;
			this.MaxSampleDate = sampleInfo.MaxSampleDate;
		}

		public DateTime? MinSampleDate { get; private set; }

		public DateTime? MaxSampleDate { get; private set; }

		public string SampleRange => (MinSampleDate.HasValue && MaxSampleDate.HasValue)
			? MinSampleDate.Value.Date.Equals(MaxSampleDate.Value.Date)
				? $"{MaxSampleDate.Value.ToShortDateString()}"
				: $"{MinSampleDate.Value.ToShortDateString()} - {MaxSampleDate.Value.ToShortDateString()}"
			: "N/A";
	}
}