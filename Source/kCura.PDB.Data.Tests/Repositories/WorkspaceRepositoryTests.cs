namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Collections.Generic;
	using global::Relativity.API;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using kCura.Relativity.Client;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class WorkspaceRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			_helper = new Mock<IHelper>();
			var client = new RSAPIClient(new Uri($"http://{Config.RSAPIServer}/relativity.services"),
				new UsernamePasswordCredentials(Config.RSAPIUsername, Config.RSAPIPassword));
			_helper.Setup(h => h.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				.Returns(client);
		}

		private Mock<IHelper> _helper;
		
		[Test]
		public void Workspaces_ReadAll()
		{
			var repo = new WorkspaceRepository(_helper.Object);
			var result = repo.ReadAll();

			Assert.That(result.Count, Is.GreaterThan(0));
		}

		[Test]
		public void Workspaces_ReadAll_Error()
		{
			var repo = new WorkspaceRepository(_helper.Object);
			Assert.Throws<Exception>(() => repo.ReadAll(new List<string>() { "asdf" }));
		}
	}
}
