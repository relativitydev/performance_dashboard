namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class EnvironmentCheckServiceTests
	{
		private EnvironmentCheckService service;
		private Mock<IEnvironmentCheckRepository> environmentCheckRepository;
		private Mock<ISqlServerRepository> sqlServerRepository;

		[SetUp]
		public void SetUp()
		{
			this.environmentCheckRepository = new Mock<IEnvironmentCheckRepository>();
			this.sqlServerRepository = new Mock<ISqlServerRepository>();
			this.sqlServerRepository.SetupGet(r => r.EnvironmentCheckRepository).Returns(this.environmentCheckRepository.Object);
			this.service = new EnvironmentCheckService(this.sqlServerRepository.Object);
		}

		[Test]
		public void ReadRecommendations()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("ID", typeof(int));
			data.Columns.Add("Status", typeof(string));
			data.Columns.Add("Scope", typeof(string));
			data.Columns.Add("Section", typeof(string));
			data.Columns.Add("Name", typeof(string));
			data.Columns.Add("Description", typeof(string));
			data.Columns.Add("Value", typeof(string));
			data.Columns.Add("Recommendation", typeof(string));
			var row = data.Rows.Add();
			row["ID"] = 1;
			row["Status"] = "abc";
			row["Scope"] = "abc";
			row["Section"] = "abc";
			row["Name"] = "abc";
			row["Description"] = "abc";
			row["Value"] = "abc";
			row["Recommendation"] = "abc";
			var row2 = data.Rows.Add();
			row2["ID"] = 2;
			row2["Status"] = DBNull.Value;
			row2["Scope"] = DBNull.Value;
			row2["Section"] = DBNull.Value;
			row2["Name"] = DBNull.Value;
			row2["Description"] = DBNull.Value;
			row2["Value"] = DBNull.Value;
			row2["Recommendation"] = DBNull.Value;

			this.environmentCheckRepository.Setup(x => x.GetRecomendations(It.IsAny<GridConditions>(), It.IsAny<EnvironmentCheckRecommendationFilterConditions>())).Returns(data);

			//Act
			var results = this.service.Recommendations(new GridConditions(), new EnvironmentCheckRecommendationFilterConditions());

			//Assert
			Assert.AreEqual(2, results.Data.Count(), "Incorrect count of scores returned");
			
		}

		

		[Test]
		public void ReadServerDetails()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("ID", typeof(Int32));
			data.Columns.Add("ServerName", typeof(String));
			data.Columns.Add("OSVersion", typeof(String));
			data.Columns.Add("OSName", typeof(String));
			data.Columns.Add("LogicalProcessors", typeof(Int32));
			data.Columns.Add("Hyperthreaded", typeof(Boolean));
			var row = data.Rows.Add();
			row["ID"] = 1;
			row["ServerName"] = "asdf Section";
			row["OSVersion"] = "asdf Section";
			row["OSName"] = "asdf Section";
			row["LogicalProcessors"] = 456;
			row["Hyperthreaded"] = true;

			this.environmentCheckRepository.Setup(x => x.GetServerDetails(It.IsAny<GridConditions>(), It.IsAny<EnvironmentCheckServerFilterConditions>(), It.IsAny<EnvironmentCheckServerFilterOperands>())).Returns(data);

			//Act
			var results = this.service.ServerDetails(new GridConditions(), new EnvironmentCheckServerFilterConditions(), new EnvironmentCheckServerFilterOperands());

			//Assert
			Assert.AreEqual(1, results.Data.Count(), "Incorrect count of scores returned");
		}

		[Test]
		public void ReadDatabaseDetails()
		{
			//Arrange
			var data = new DataTable();
			data.Columns.Add("ID", typeof(Int32));
			data.Columns.Add("ServerName", typeof(String));
			data.Columns.Add("SQLVersion", typeof(String));
			data.Columns.Add("AdHocWorkLoad", typeof(Int32));
			data.Columns.Add("MaxServerMemory", typeof(float));
			data.Columns.Add("MaxDegreeOfParallelism", typeof(Int32));
			data.Columns.Add("TempDBDataFiles", typeof(Int32));
			data.Columns.Add("LastSQLRestart", typeof(DateTime));
			var row = data.Rows.Add();
			row["ID"] = 1;
			row["ServerName"] = "asdf Section";
			row["SQLVersion"] = "asdf Name";
			row["AdHocWorkLoad"] = 345;
			row["MaxServerMemory"] = 8589934.592;
			row["MaxDegreeOfParallelism"] = 0;
			row["TempDBDataFiles"] = 123;
			row["LastSQLRestart"] = DateTime.Now;

			this.environmentCheckRepository.Setup(x => x.GetDatabaseDetails(It.IsAny<GridConditions>(), It.IsAny<EnvironmentCheckDatabaseFilterConditions>(), It.IsAny<EnvironmentCheckDatabaseFilterOperands>())).Returns(data);

			//Act
			var results = this.service.DatabaseDetails(new GridConditions(), new EnvironmentCheckDatabaseFilterConditions(), new EnvironmentCheckDatabaseFilterOperands());

			//Assert
			Assert.AreEqual(1, results.Data.Count(), "Incorrect count of scores returned");
		}
	}
}
