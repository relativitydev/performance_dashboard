namespace kCura.PDB.Core.Extensions
{
	using System;
	using kCura.PDB.Core.Models;

	public static class HourExtensions
	{
		public static DateTime GetHourEnd(this Hour hour)
		{
			return hour.GetNextHour().AddMilliseconds(-1);
		}

		public static DateTime GetNextHour(this Hour hour)
		{
			return hour.HourTimeStamp.AddHours(1);
		}
	}
}
