namespace kCura.PDB.Data.Tests.Services
{
	using System;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Data;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class CachedConnectionStringFactoryTests
	{
		[SetUp]
		public void Setup()
		{
			this.connectionFactory1 = new MockConnectionFactory();
			this.connectionFactory2 = new MockConnectionFactory();
		}

		private IConnectionFactory connectionFactory1;
		private IConnectionFactory connectionFactory2;

		[Test]
		public void CachedConnectionStringFactory_GetEddsPerformanceConnection()
		{
			var result1 = this.connectionFactory1.GetEddsPerformanceConnection();
			var result2 = this.connectionFactory1.GetEddsPerformanceConnection();
			Assert.That(result1.ConnectionString, Is.EqualTo(result2.ConnectionString));
		}

		[Test]
		[TestCase("bob", "bob", true, true, true)]
		[TestCase("bob", "not bob", true, true, false)]
		[TestCase("bob", "bob", false, true, false)]
		[TestCase("bob", "not bob", true, true, false)]
		public void CachedConnectionStringFactory_GetEddsConnection(string username1, string username2, bool useWinAuth1, bool useWinAuth2, bool expectedResult)
		{
			var result1 = this.connectionFactory1.GetEddsConnection(new GenericCredentialInfo { UserName = username1, UseWindowsAuthentication = useWinAuth1, Password = "123" });
			var result2 = this.connectionFactory2.GetEddsConnection(new GenericCredentialInfo { UserName = username2, UseWindowsAuthentication = useWinAuth2, Password = "123" });
			Assert.That(result1.ConnectionString == result2.ConnectionString, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(true, true, true)]
		[TestCase(false, true, false)]
		public void CachedConnectionStringFactory_GetEddsConnection_NullCredInfo(bool credIsNull1, bool credIsNull2, bool expectedResult)
		{
			var result1 = this.connectionFactory1.GetEddsConnection(credIsNull1 ? null : new GenericCredentialInfo { UserName = "bob", Password = "123" });
			var result2 = this.connectionFactory2.GetEddsConnection(credIsNull2 ? null : new GenericCredentialInfo { UserName = "bob", Password = "123" });
			Assert.That(result1.ConnectionString == result2.ConnectionString, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase("eddsperf", "eddsperf", "server", "server", "bob", "bob", true)]
		[TestCase("eddsperf 1", "eddsperf 2", "server", "server", "bob", "bob", false)]
		[TestCase("eddsperf", "eddsperf", "server 1", "server 2", "bob", "bob", false)]
		[TestCase("eddsperf", "eddsperf", "server", "server", "bob 1", "bob 2", false)]
		public void CachedConnectionStringFactory_GetTargetConnectionString_NullCredInfo(string db1, string db2, string server1, string server2, string username1, string username2, bool expectedResult)
		{
			var result1 = this.connectionFactory1.GetTargetConnectionString(db1, server1, new GenericCredentialInfo { UserName = username1, Password = "123" });
			var result2 = this.connectionFactory2.GetTargetConnectionString(db2, server2, new GenericCredentialInfo { UserName = username2, Password = "123" });
			Assert.That(result1 == result2, Is.EqualTo(expectedResult));
		}

		private class MockConnectionFactory : CachedConnectionStringFactory
		{
			public MockConnectionFactory()
				: base(new MockWorkspaceServerProvider())
			{
			}

			protected override SqlConnectionStringBuilder GetConnectionString(string server = null, GenericCredentialInfo credentialInfo = null)
			{
				return new SqlConnectionStringBuilder
				{
					DataSource = server ?? "test server",
					ApplicationName = Guid.NewGuid().ToString() // Ensures multiple calls to this method will result in different connection strings
				}
				.ModifyCreditentals(credentialInfo);
			}

			private class MockWorkspaceServerProvider : IWorkspaceServerProvider
			{
				public string GetWorkspaceServer(int workspaceId)
				{
					return "primary-sql-server";
				}
			}
		}
	}
}
