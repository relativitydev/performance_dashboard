namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Service.BISSummary;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class FileLatencyServiceTests
	{
		[SetUp]
		public void SetUp()
		{
			this.fileLatencyRepository = new Mock<IFileLatencyRepository>();
			this.sqlServerRepository = new Mock<ISqlServerRepository>();
			this.sqlServerRepository.SetupGet(r => r.FileLatencyRepository).Returns(this.fileLatencyRepository.Object);
			this.service = new FileLatencyService(this.sqlServerRepository.Object);
		}

		private Mock<IFileLatencyRepository> fileLatencyRepository;
		private Mock<ISqlServerRepository> sqlServerRepository;
		private FileLatencyService service;

		[Test]
		public void ReadFileLatencies()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("ServerName", typeof(String));
			data.Columns.Add("DatabaseName", typeof(String));
			data.Columns.Add("Score", typeof(decimal));
			data.Columns.Add("DataReadLatency", typeof(long));
			data.Columns.Add("DataWriteLatency", typeof(long));
			data.Columns.Add("LogReadLatency", typeof(long));
			data.Columns.Add("LogWriteLatency", typeof(long));
			var row = data.Rows.Add();
			row["ServerName"] = "localhost";
			row["DatabaseName"] = "TestDb1";
			row["Score"] = 22.5;
			row["DataReadLatency"] = 123;
			row["DataWriteLatency"] = 456;
			row["LogReadLatency"] = 789;
			row["LogWriteLatency"] = 012;
			var row2 = data.Rows.Add();
			row2["ServerName"] = "localhost";
			row2["DatabaseName"] = "TestDb2";
			row2["Score"] = 75.1;
			row2["DataReadLatency"] = 333;
			row2["DataWriteLatency"] = 555;
			row2["LogReadLatency"] = 777;
			row2["LogWriteLatency"] = 999;

			this.fileLatencyRepository.Setup(x => x.GetFileLevelLatencyDetails(
				It.IsAny<GridConditions>(),
				It.IsAny<Dictionary<FileLatency.Columns, String>>(),
				It.IsAny<Dictionary<FileLatency.Columns, FilterOperand>>()))
				.Returns(data);

			//Act
			var results = this.service.FileLatencies(new GridConditions(), new Dictionary<FileLatency.Columns, String>(), new Dictionary<FileLatency.Columns, FilterOperand>());

			//Assert
			Assert.AreEqual(2, results.Data.Count(), "Incorrect count of scores returned");

		}
	}
}
