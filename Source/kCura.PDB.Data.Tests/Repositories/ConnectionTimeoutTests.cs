namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class ConnectionTimeoutTests
	{
		[SetUp]
		public void SetUp()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			DataSetup.Setup();
		}

		private IConnectionFactory connectionFactory;

		private async Task<int> ReadValue(TimeSpan wait)
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				return await conn.QueryFirstOrDefaultAsync<int>($"WAITFOR DELAY '{wait.Hours}:{wait.Minutes}:{wait.Seconds}';select 1");
			}
		}

		[Test]
		[Category("Explicit")]
		[Explicit("Takes a long time")]
		public void Timeout_GreaterThanTimeout()
		{
			// Arrange
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				// Act
				var exception = Assert.ThrowsAsync<System.Data.SqlClient.SqlException>(() => ReadValue(TimeSpan.FromSeconds(Defaults.Database.ConnectionTimeout + 3)));
				stopwatch.Stop();

				// Assert
				Assert.That(exception.Message.Contains("Execution Timeout Expired"), Is.True);
				Assert.That(stopwatch.Elapsed.TotalSeconds, Is.GreaterThan(Defaults.Database.ConnectionTimeout));
			}
			finally
			{
				Console.WriteLine($"Total time: {stopwatch.Elapsed.TotalSeconds}");
			}

		}

		[Test]
		[Category("Explicit")]
		[Explicit("Takes a long time")]
		public async Task Timeout_LessThanTimeout()
		{
			// Arrange
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				// Act
				var value = await ReadValue(TimeSpan.FromSeconds(Defaults.Database.ConnectionTimeout - 3));
				stopwatch.Stop();

				// Assert
				Assert.That(value, Is.EqualTo(1));
				Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(Defaults.Database.ConnectionTimeout));
			}
			finally
			{
				Console.WriteLine($"Total time: {stopwatch.Elapsed.TotalSeconds}");
			}
		}

		[Test]
		public async Task Timeout_LessThanTimeout_SmallWait()
		{
			// Arrange
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				// Act
				var value = await ReadValue(TimeSpan.FromSeconds(1));
				stopwatch.Stop();

				Assert.That(value, Is.EqualTo(1));
				Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(Defaults.Database.ConnectionTimeout));
			}
			finally
			{
				Console.WriteLine($"Total time: {stopwatch.Elapsed.TotalSeconds}");
			}
		}
	}
}
