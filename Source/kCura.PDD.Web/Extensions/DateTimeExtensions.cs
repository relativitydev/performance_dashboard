using System;

namespace kCura.PDD.Web.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first day of the week that the specified date 
		/// is in. 
		/// </summary>
		public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek, DayOfWeek firstDay)
		{
				DateTime firstDayInWeek = dayInWeek.Date;
				while (firstDayInWeek.DayOfWeek != firstDay)
						firstDayInWeek = firstDayInWeek.AddDays(-1);

				return firstDayInWeek;
		}

		/// <summary>
		/// Returns the last day of the week that the specified date 
		/// is in. 
		/// </summary>
		public static DateTime GetLastDayOfWeek(this DateTime dayInWeek, DayOfWeek lastDay)
		{
			DateTime firstDayInWeek = dayInWeek.Date;
			while (firstDayInWeek.DayOfWeek != lastDay)
				firstDayInWeek = firstDayInWeek.AddDays(1);

			return firstDayInWeek;
		}

	    public static DateTime NormalizeHour(this DateTime dateTime)
	    {
            return
                dateTime.Subtract(new TimeSpan(0, dateTime.Minute, dateTime.Second))
                    .AddMilliseconds(-dateTime.Millisecond);
        }
	}
}