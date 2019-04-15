namespace kCura.PDB.Core.Models.Audits
{
	using System.Collections.Generic;

	public class PastWeekEligibleSample
	{
		public int ServerId;
		public int HourId;
		public IList<SampleHistory> ArrivalRateHours;
		public IList<SampleHistory> ConcurrencyHours;

		public PastWeekEligibleSample()
		{
			ArrivalRateHours = new List<SampleHistory>();
			ConcurrencyHours = new List<SampleHistory>();
		}
	}
}
