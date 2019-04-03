using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace kCura.PDB.TrustBridge
{
    class Bridge
    {
        #region Constants
        private const string _header = "InstanceKey,InstanceName,Datacenter,ClientName,Enabled,ScoreDate,QuarterlyScore,WeeklyScore,SampleStartHour,SampleEndHour,TimeStamp,MissingHours,FailingCaseHours,CompletedCaseHours,LastGlassRunTime,ApplicationVersion,DbccMonitoringEnabled,DbccMissedDays";

        private const string _csvLineFormat = "{0},\"{1}\",\"{2}\",\"{3}\",{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}";

        private const string _sql =
            @"SELECT
	            [InstanceKey],
	            [InstanceName],
	            [Datacenter],
	            [ClientName],
	            [Enabled],
	            [ScoreDate],
	            [QuarterlyScore],
	            [WeeklyScore],
	            [SampleStartHour],
	            [SampleEndHour],
	            [TimeStamp],
	            [MissingHours],
	            [FailingCaseHours],
	            [CompletedCaseHours],
	            [LastGlassRunTime],
	            [ApplicationVersion],
	            [DbccMonitoringEnabled],
	            [DbccMissedDays]
            FROM [Trust].[dbo].[RecentMetricsSummary] WITH(NOLOCK)";
        #endregion

        #region Methods
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.AppSettings["connectionString"];
            var outputPath = ConfigurationManager.AppSettings["outputFilePath"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(outputPath))
            {
                Console.WriteLine("ERROR: Terminating due to missing/invalid configuration settings");
                return;
            }

            if (File.Exists(outputPath))
            {
                Console.WriteLine("Deleting existing CSV...");
                File.Delete(outputPath);
            }

            Console.WriteLine("Reading metrics...");

            var metrics = ReadMetrics(connectionString);
            var sb = new StringBuilder();
            sb.AppendLine(_header);
            foreach (var m in metrics)
            {
                sb.AppendLine(string.Format(_csvLineFormat,
                    m.InstanceKey,
                    m.InstanceName,
                    m.DataCenter,
                    m.ClientName,
                    m.Enabled ? "1" : "0",
                    m.ScoreDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    m.QuarterlyScore,
                    m.WeeklyScore,
                    m.SampleStartDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    m.SampleEndDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    m.MissingHours,
                    m.FailingCaseHours,
                    m.CompletedCaseHours,
                    m.LastGlassRunTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    m.ApplicationVersion,
                    m.DbccMonitoringEnabled ? "1" : "0",
                    m.DbccMissedDays
                ));
            }

            Console.WriteLine("Generating CSV...");
            File.WriteAllText(outputPath, sb.ToString());
            Console.WriteLine("Done!");
        }

        private static List<MetricInfo> ReadMetrics(string connectionString)
        {
            var metrics = new List<MetricInfo>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = _sql;
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var metric = new MetricInfo();
                            metric.InstanceKey = r.GetString(0);
                            metric.InstanceName = r.GetString(1);
                            metric.DataCenter = r.GetString(2);
                            metric.ClientName = r.GetString(3);
                            metric.Enabled = r.GetBoolean(4);
                            metric.ScoreDate = r.GetDateTime(5);
                            metric.QuarterlyScore = r.GetDecimal(6);
                            metric.WeeklyScore = r.GetDecimal(7);
                            metric.SampleStartDate = r.IsDBNull(8) ? metric.ScoreDate.AddDays(-90) : r.GetDateTime(8);
                            metric.SampleEndDate = r.IsDBNull(9) ? metric.ScoreDate : r.GetDateTime(9);
                            metric.Timestamp = r.GetDateTime(10);
                            metric.MissingHours = r.IsDBNull(11) ? 0 : r.GetInt32(11);
                            metric.FailingCaseHours = r.GetInt32(12);
                            metric.CompletedCaseHours = r.GetInt32(13);
                            metric.LastGlassRunTime = r.GetDateTime(14);
                            metric.ApplicationVersion = r.GetString(15);
                            metric.DbccMonitoringEnabled = r.IsDBNull(16) ? true : r.GetBoolean(16);
                            metric.DbccMissedDays = r.IsDBNull(17) ? 0 : r.GetInt32(17);
                            metrics.Add(metric);
                        }
                    }
                }
            }
            return metrics;
        }
        #endregion
    }
}
