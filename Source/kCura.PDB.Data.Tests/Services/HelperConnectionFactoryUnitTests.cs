namespace kCura.PDB.Data.Tests.Services
{
	using System.Data.SqlClient;
	using global::Relativity.API;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Data.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class HelperConnectionFactoryUnitTests
	{
		private Mock<IHelper> helperMock;
		private HelperConnectionFactory connectionFactory;

		[SetUp]
		public void Setup()
		{
			this.helperMock = new Mock<IHelper>();
			this.connectionFactory = new HelperConnectionFactory(this.helperMock.Object);
		}

		[Test]
		[Ignore("Cannot test HelperConnectionFactory because of the GenericConnectionFactory.GetDefaultConnectionString dependency")]
		public void GetWorkspaceConnection_Success()
		{
			// Arrange
			var workspaceId = 12345;
			var workspaceServer = "WorkspaceServer";
			var eddsServer = "EddsServer";
			var workspaceDb = string.Format(Names.Database.EddsWorkspacePrefix, workspaceId);
			var eddsDbContextMock = new Mock<IDBContext>();
			eddsDbContextMock.Setup(m => m.ServerName).Returns(eddsServer);
			var workspaceDbContextMock = new Mock<IDBContext>();
			workspaceDbContextMock.Setup(m=>m.ServerName).Returns(workspaceServer);
			workspaceDbContextMock.Setup(m => m.Database).Returns(workspaceDb);

			this.helperMock.Setup(m => m.GetDBContext(-1)).Returns(eddsDbContextMock.Object);
			this.helperMock.Setup(m => m.GetDBContext(workspaceId)).Returns(workspaceDbContextMock.Object);

			// Act
			using (var result = this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				// Assert
				Assert.That(result.Database, Is.EqualTo(workspaceDb));
				var conStringBuilder = new SqlConnectionStringBuilder(result.ConnectionString);
				Assert.That(conStringBuilder.DataSource, Is.EqualTo(workspaceServer));
			}
		}
	}
}
