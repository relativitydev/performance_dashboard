using System;
using System.Collections.Generic;

namespace kCura.PDD.Web.Models.BISSummary
{
    public class UptimeViewModel
    {
				public double UptimePercentage;
				public double WeeklyUptimePercentage;
				public int UptimeScore;
				public int WeeklyUptimeScore;
				public IList<UptimeValue> HourlyOutages;
				public IList<UptimeValue> DailyData;

        public UptimeViewModel()
        {
            HourlyOutages = new List<UptimeValue>();
			DailyData = new List<UptimeValue>();
        }
    }

    public class UptimeValue
    {
        public DateTime Date { get; set; }
        public double Uptime { get; set; }
		public double HoursDown { get; set; }
		public int Score { get; set; }
    }
}