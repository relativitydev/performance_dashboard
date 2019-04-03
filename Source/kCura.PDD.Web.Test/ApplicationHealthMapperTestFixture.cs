namespace kCura.PDD.Web.Test
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using kCura.PDB.Core.Models.HealthChecks;
	using kCura.PDB.Service.HealthChecks;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;
	using kCura.PDD.Web.Mapper;

	[TestFixture, Category("Integration"), Category("Ignore")]
	public class ApplicationHealthMapperTestFixture
	{
		[Test, Ignore]
		public void RunApplicationHealthMapper_Success()
		{
			IList<HealthBase> healthData;
			var task = new HealthTask();
			healthData = task.GetHealthData(MeasureTypes.AppHealth, DateTime.Now.AddDays(-90), DateTime.Now.AddDays(1), 300);
			var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);
			String tempPath = System.IO.Path.GetTempFileName();

			using (var file = System.IO.File.Open(tempPath, System.IO.FileMode.Append))
			{
				foreach (var appHealthData in applicationHealthDatas)
				{
					var bytes = Encoding.ASCII.GetBytes(String.Format("{0},{1},{2},{3},{4},{5},{6}\r\n", appHealthData.SQLInstanceName, appHealthData.Errors, appHealthData.ArtifactID, appHealthData.LRQ, appHealthData.MeasureDate, appHealthData.Users, appHealthData.WorkspaceName));
					file.Write(bytes, 0, bytes.Length);
				}
			}
			Console.WriteLine(tempPath);
		}

		[Test, Ignore]
		public void RunApplicationHealthMapper_PeakUserFeature()
		{
			IList<HealthBase> healthData;
			var task = new HealthTask();
			healthData = task.GetHealthData(MeasureTypes.AppHealth, DateTime.Now.AddDays(-90), DateTime.Now.AddDays(1), 300);
			var applicationHealthDatas = ApplicationHealthMapper.ToApplicationHealth(healthData);


			Assert.IsTrue(false);
		}



	}
}
