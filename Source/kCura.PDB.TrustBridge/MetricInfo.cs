using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.PDB.TrustBridge
{
    public class MetricInfo
    {
        public string InstanceKey;
        public string InstanceName;
        public string DataCenter;
        public string ClientName;
        public bool Enabled;
        public DateTime ScoreDate;
        public decimal QuarterlyScore;
        public decimal WeeklyScore;
        public DateTime SampleStartDate;
        public DateTime SampleEndDate;
        public DateTime Timestamp;
        public int MissingHours;
        public int FailingCaseHours;
        public int CompletedCaseHours;
        public DateTime LastGlassRunTime;
        public string ApplicationVersion;
        public bool DbccMonitoringEnabled;
        public int DbccMissedDays;
    }
}
