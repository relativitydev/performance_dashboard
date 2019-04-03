namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories.BISSummary;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class AnalyticsRepositoryTests
	{
		public void SetUp()
		{
			this.connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
		}

		private IConnectionFactory connectionFactory;

		[Test, Category("Integration")]
		public void SaveAnalyticsRecommendation()
		{
			//Arrange
			var ecRepo = new EnvironmentCheckRepository(connectionFactory);
			var aRepo = new AnalyticsRepository(connectionFactory, ecRepo);

			//Act
			aRepo.SaveAnalyticsRecommendation(new Server() { ServerName = "localhost" }, Guid.Parse("AB10EDAE-67F9-4636-A4C6-94D7EF20D705"), "asdf");

			//Assert

		}

		[Test, Category("Integration")]
		public void ReadCaatPopTables()
		{
			//Arrange
			var aRepo = new AnalyticsRepository(connectionFactory, null);

			//Act
			var result = aRepo.ReadCaatPopTables(Config.WorkSpaceId);

			result.ToList();
			result.ToList();
			result.ToList();
			result.ToList();
			result.ToList();

			//Assert
			Assert.That(result.Count(), Is.GreaterThan(0));
		}

		[Test, Category("Integration")]
		public void ReadCaatIndexes()
		{
			// TODO -- ? Do these need to target workspace?
			//Arrange
			var aRepo = new AnalyticsRepository(connectionFactory, null);

			//Act
			var result = aRepo.ReadCaatIndexes(Config.WorkSpaceId);

			//Assert
			Assert.That(result.Count(), Is.GreaterThan(0));
		}

		[Test, Category("Integration")]
		public void ReadCaatSearchableDocuments()
		{
			//Arrange
			var aRepo = new AnalyticsRepository(connectionFactory, null);

			//Act
			var caatPopTables = aRepo.ReadCaatPopTables(Config.WorkSpaceId);
			var indexes = aRepo.ReadCaatIndexes(Config.WorkSpaceId);
			var result1 = aRepo.ReadCaatSearchableDocuments(Config.WorkSpaceId, caatPopTables, indexes);
			var result2 = aRepo.ReadCaatTrainingDocuments(Config.WorkSpaceId, caatPopTables, indexes);

			//Assert
			Assert.That(result1.Count, Is.GreaterThan(0));
			Assert.That(result2.Count, Is.GreaterThan(0));
		}

		[Test, Category("Integration")]
		public void ReadCaatSearchableDocuments_NoCaatPopTables()
		{
			//Arrange
			var aRepo = new AnalyticsRepository(connectionFactory, null);

			//Act
			var caatPopTables = new List<string>();
			var indexes = aRepo.ReadCaatIndexes(Config.WorkSpaceId);
			var result1 = aRepo.ReadCaatSearchableDocuments(Config.WorkSpaceId, caatPopTables, indexes);
			var result2 = aRepo.ReadCaatTrainingDocuments(Config.WorkSpaceId, caatPopTables, indexes);

			//Assert
			Assert.That(result1.Count, Is.EqualTo(0));
			Assert.That(result2.Count, Is.EqualTo(0));
		}

		[Test, Category("Integration")]
		public void ReadCaatSearchableDocuments_NoCaatIndexes()
		{
			//Arrange
			var aRepo = new AnalyticsRepository(connectionFactory, null);

			//Act
			var caatPopTables = aRepo.ReadCaatPopTables(Config.WorkSpaceId);
			var indexes = new List<Int32>();
			var result1 = aRepo.ReadCaatSearchableDocuments(Config.WorkSpaceId, caatPopTables, indexes);
			var result2 = aRepo.ReadCaatTrainingDocuments(Config.WorkSpaceId, caatPopTables, indexes);

			//Assert
			Assert.That(result1.Count, Is.EqualTo(0));
			Assert.That(result2.Count, Is.EqualTo(0));
		}


		[Test, Category("Unit")]
		public void ReadCaatSearchableDocuments_NoCaatIndexesOrPopTables()
		{
			//Arrange
			var workspaceId = -343;
			var connectionFactoryMock = new Mock<IConnectionFactory>();
			var aRepo = new AnalyticsRepository(connectionFactoryMock.Object, null);

			//Act
			var caatPopTables = new List<string>();
			var indexes = new List<Int32>();
			var result1 = aRepo.ReadCaatSearchableDocuments(workspaceId, caatPopTables, indexes);
			var result2 = aRepo.ReadCaatTrainingDocuments(workspaceId, caatPopTables, indexes);

			//Assert
			Assert.That(result1.Count, Is.EqualTo(0));
			Assert.That(result2.Count, Is.EqualTo(0));
		}

		[Test, Category("Unit")]
		public void ReadCaatSearchableDocuments_FilterOutBadPopTables()
		{
			//Arrange
			var workspaceId = -343;
			var connectionFactoryMock = new Mock<IConnectionFactory>();
			var aRepo = new AnalyticsRepository(connectionFactoryMock.Object, null);

			//Act
			var caatPopTables = new List<string>();
			var badPopTables = new[] { "asdf", "Zca_POP_2", "Zca_POP_asdf_123" };
			var popTablesDontMatchIndexes = new[] { "Zca_POP_999_123", "Zca_POP_123_123" };
			caatPopTables.AddRange(badPopTables);
			caatPopTables.AddRange(popTablesDontMatchIndexes);
			var indexes = new List<Int32>() { 1, 2 };
			var result1 = aRepo.ReadCaatSearchableDocuments(workspaceId, caatPopTables, indexes);
			var result2 = aRepo.ReadCaatTrainingDocuments(workspaceId, caatPopTables, indexes);

			//Assert
			Assert.That(result1.Count, Is.EqualTo(0));
			Assert.That(result2.Count, Is.EqualTo(0));
		}
	}
}