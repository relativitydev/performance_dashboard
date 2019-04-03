namespace kCura.PDB.Service.HealthChecks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.HealthChecks;
	using kCura.PDB.Core.Models.HealthChecks;
	using kCura.PDB.Data;

	public class HealthTask : IHealthTask
	{
		public IList<HealthBase> GetHealthData(MeasureTypes measureType, DateTime? startDate, DateTime? endDate, int timeZoneOffset)
		{
			List<HealthBase> list = null;

			if (startDate.HasValue && !endDate.HasValue)
			{
				return list;
			}

			if (endDate.HasValue && (!startDate.HasValue))
			{
				return list;
			}

			if (startDate != null && startDate.Value > endDate.Value)
			{
				return list;
			}

			switch (measureType)
			{
				case MeasureTypes.AppHealth:
					return GetApplicationHealthData(startDate, endDate, timeZoneOffset);
				case MeasureTypes.Ram:
					return GetRamHealthData(startDate, endDate, timeZoneOffset);
				case MeasureTypes.HardDisk:
					return GetHardDiskData(startDate, endDate, timeZoneOffset);
				case MeasureTypes.Processor:
					return GetProcessorData(startDate, endDate, timeZoneOffset);
				case MeasureTypes.SQLServer:
					return GetSQLServerData(startDate, endDate, timeZoneOffset);
				default:
					break;
			}

			return list;
		}

		/// <summary>
		/// Get field list for filter display column
		/// </summary>
		/// <param name="measureType">The measure type</param>
		/// <returns>List of key value pairs</returns>
		public IList<KeyValue> GetColumnsList(MeasureTypes measureType)
		{
			using (var context = new PDDModelDataContext())
			{
				return (from m in context.Measures
						where m.MeasureTypeId == (int)measureType && m.IsDeleted != true
						select new KeyValueDesc()
						{
							Key = m.MeasureID,
							Value = m.MeasureCd,
							Description = m.MeasureDesc
						}).OfType<KeyValue>().ToList();
			}
		}

		/// <summary>
		/// Get the application health data
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <param name="timeZoneOffset">The timezone offset</param>
		/// <returns>A list of health tasks</returns>
		private IList<HealthBase> GetApplicationHealthData(DateTime? startDate, DateTime? endDate, int timeZoneOffset)
		{
			using (var context = new PDDModelDataContext())
			{
				return (from item in context.GetApplicationHealthData(startDate, endDate, timeZoneOffset)
						select new ApplicationHealth()
						{
							Id = Convert.ToInt32(item.Id),
							CaseArtifactId = item.CaseArtifactID,
							ErrorCount = item.ErrorCount,
							LRQCount = item.LRQCount,
							UserCount = item.UserCount,
							WorkspaceName = item.WorkspaceName,
							DatabaseLocation = item.DatabaseLocation,
							MeasureDate = item.MeasureDate.Value,
						}).OfType<HealthBase>().ToList();
			}
		}

		/// <summary>
		/// Get ram heath data list
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <param name="timeZoneOffset">The timezone offset</param>
		/// <returns>A list of health tasks</returns>
		private IList<HealthBase> GetRamHealthData(DateTime? startDate, DateTime? endDate, int timeZoneOffset)
		{
			using (var context = new PDDModelDataContext())
			{

				return (from item in context.GetRAMHealthData(startDate, endDate, timeZoneOffset)
						select new RamHealth()
						{
							Id = item.ServerID,
							Server = item.ServerName,
							ServerType = item.ServerTypeName,
							MeasureDate = item.MeasureDate.Value,
							PagesPerSec = item.RAMPagesPerSec,
							PageFaultsPerSec = item.RAMPageFaultsPerSec,
							Percentage = item.RAMPct
						}).OfType<HealthBase>().ToList();

			}
		}

		/// <summary>
		/// Get sql server data list
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <param name="timeZoneOffset">The timezone offset</param>
		/// <returns>A list of health tasks</returns>
		private IList<HealthBase> GetSQLServerData(DateTime? startDate, DateTime? endDate, int timeZoneOffset)
		{
			using (var context = new PDDModelDataContext())
			{
				return (from item in context.GetSQLServerSummaryData(startDate, endDate, timeZoneOffset)
						select new SqlServerHealth()
						{
							Id = item.ServerID,
							Server = item.ServerName,
							ServerType = item.ServerTypeName,
							PageLifeExpectancy = item.SQLPageLifeExpectancy,
							MeasureDate = item.MeasureDate.Value
						}).OfType<HealthBase>().ToList();
			}
		}

		/// <summary>
		/// Get processor data list
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <param name="timeZoneOffset">The timezone offset</param>
		/// <returns>A list of health tasks</returns>
		private IList<HealthBase> GetProcessorData(DateTime? startDate, DateTime? endDate, int timeZoneOffset)
		{
			using (var context = new PDDModelDataContext())
			{
				return (from item in context.GetServerProcessorSummaryData(startDate, endDate, timeZoneOffset)
						select new ProcesserHealth()
						{
							Id = Convert.ToInt32(item.ServerCoreId),
							Server = item.ServerName,
							ServerType = item.ServerTypeName,
							ServerCore = item.ServerName,
							CPUProcessorTime = item.CPUProcessorTimePct,
							ProcesserTime = item.CPUProcessorTimePct,
							MeasureDate = Convert.ToDateTime(item.MeasureDate)
						}).OfType<HealthBase>().ToList();
			}
		}

		/// <summary>
		/// Get disk data list
		/// </summary>
		/// <param name="startDate">The start date</param>
		/// <param name="endDate">The end date</param>
		/// <param name="timeZoneOffset">The timezone offset</param>
		/// <returns>A list of health tasks</returns>
		private IList<HealthBase> GetHardDiskData(DateTime? startDate, DateTime? endDate, int timeZoneOffset)
		{
			using (var context = new PDDModelDataContext())
			{
				return (from item in context.GetServerDiskSummaryData(startDate, endDate, timeZoneOffset)
						select new HardDiskHealth()
						{
							Id = Convert.ToInt32(item.ServerDiskId),
							Server = item.ServerName,
							ServerType = item.ServerTypeName,
							ServerDisk = item.ServerName + "-" + item.DiskNumber.ToString(),
							DiskRead = item.DiskAvgSecPerRead,
							DiskWrite = item.DiskAvgSecPerWrite,
							DiskNumber = item.DiskNumber,
							DriveLetter = item.DriveLetter,
							MeasureDate = item.MeasureDate.Value
						}).OfType<HealthBase>().ToList();
			}
		}
	}
}