namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.ScriptInstallation;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class SqlScriptTokenValueProviderTests
	{
		[SetUp]
		public void Setup()
		{
			_sqlRepo = new Mock<IDeploymentRepository>();
		}

		private Mock<IDeploymentRepository> _sqlRepo;

		[Test]
		public void GetValue_TokenDoesntExist()
		{
			//Arrange

			//Act
			var srv = new SqlScriptTokenValueProvider(_sqlRepo.Object, "");
			Assert.Throws<KeyNotFoundException>(() => srv.GetValue("blah"));
		}

		[TestCase(SqlScriptTokens.MdfDir, @"c:\somepath\" + SqlScriptTokens.MdfDir)]
		[TestCase(SqlScriptTokens.LdfDir, @"c:\somepath\" + SqlScriptTokens.LdfDir)]
		[TestCase(SqlScriptTokens.Collation, @"SQL_Latin1_General_CP1_CI_AS")]
		public void GetValue_Mdf(string key, string expectedValue)
		{
			//Arrange
			var path = @"c:\somepath\" + key;
			var collation = @"SQL_Latin1_General_CP1_CI_AS";
			_sqlRepo.Setup(r => r.ReadMdfLdfDirectories(It.IsAny<String>()))
				.Returns(new DatabaseDirectoryInfo() { MdfPath = path, LdfPath = path });
			_sqlRepo.Setup(r => r.ReadCollationSettings())
				.Returns(collation);

			//Act
			var srv = new SqlScriptTokenValueProvider(_sqlRepo.Object, "");
			var result = srv.GetValue(key);

			//Assert
			Assert.That(result, Is.EqualTo(expectedValue));
		}

		[TestCase(SqlScriptTokens.MdfDir, @"c:\somepath\1\" + SqlScriptTokens.MdfDir)]
		[TestCase(SqlScriptTokens.LdfDir, @"c:\somepath\1\" + SqlScriptTokens.LdfDir)]
		[TestCase(SqlScriptTokens.Collation, @"1")]
		public void GetValue_MdfCached(string key, string expectedValue)
		{
			//Arrange
			var path = @"c:\somepath\{0}\" + key;
			var collation = @"{0}";
			var i1 = 0;
			var i2 = 0;

			//mock returns a path that increments with each time it's called
			//so if the value is not cached then it will call this again to reload value, thus incrementing return value
			_sqlRepo.Setup(r => r.ReadMdfLdfDirectories(It.IsAny<String>()))
				.Returns(() => new DatabaseDirectoryInfo { MdfPath = String.Format(path, i1), LdfPath = String.Format(path, i1++) });
			_sqlRepo.Setup(r => r.ReadCollationSettings())
				.Returns(() => String.Format(collation, i2++));

			//artificially increment
			_sqlRepo.Object.ReadMdfLdfDirectories("");
			_sqlRepo.Object.ReadCollationSettings(); 

			//Act
			var srv = new SqlScriptTokenValueProvider(_sqlRepo.Object, "");
			srv.GetValue(key); //first time to load value
			var result = srv.GetValue(key); //read cached value

			//Assert
			Assert.That(result, Is.EqualTo(expectedValue));
		}

		
	}
}
