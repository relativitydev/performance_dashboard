namespace kCura.PDB.Service.Testing
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Ganss.Excel;

	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models.Testing;

	public class TestDataExcelParser : ITestDataExcelParser
	{
		private ExcelMapper SetupMapper(byte[] excelData)
		{
			// Import excel sheet using tool/service
			var stream = new MemoryStream(excelData);
			return new ExcelMapper(stream);
		}

		public TestBackupDbccData ParseExcelBackupDbccData(byte[] excelData, bool throwOnNullData = false)
		{
			var mapper = this.SetupMapper(excelData);

			var data = new TestBackupDbccData
				           {
					           // Fetch data
					           Hours = mapper.Fetch<MockHour>(Names.Testing.HourSheet).ToList(),
					           DatabasesChecked = mapper.Fetch<MockDatabaseChecked>(Names.Testing.DatabasesCheckedSheet).ToList(),
					           Servers = mapper.Fetch<MockServer>(Names.Testing.ServerSheet).ToList()
				           };

			try
			{
				data.Backups = mapper.Fetch<MockBackupSet>(Names.Testing.BackupSheet).ToList();
			}
			catch (NullReferenceException)
			{
				data.Backups = new List<MockBackupSet>();

				// May not be relevant for test
				if (throwOnNullData) throw;
			}

			try
			{
				data.DbccResults = mapper.Fetch<MockDbccServerResults>(Names.Testing.DbccSheet).ToList();
			}
			catch (NullReferenceException)
			{
				data.DbccResults = new List<MockDbccServerResults>();

				// May not be relevant for test
				if (throwOnNullData) throw;
			}

			return data;
		}
	}
}
