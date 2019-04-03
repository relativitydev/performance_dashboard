namespace kCura.PDB.Core.Extensions
{
	using System;

	public static class DateTimeExtensions
	{
		public static DateTime NormilizeToHour(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, 0, 0);
		}
	}
}
