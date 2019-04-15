namespace kCura.PDB.Service.Tests.ProcessControls
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck;
	using kCura.PDB.Tests.Common;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Integration"), Category("Ignore"), Ignore("Connection string usage needs to be re-evaluated")]
	public class EnvironmentCheckServerInfoTaskTests
	{
		[Test]
		public void ExecuteEnvironmentCheckServerInfo_SaveServerDetails()
		{
			//Arrange
			var logger = TestUtilities.GetMockLogger();

			// TODO -- Connection string refactor
			var sqlRepo = new Mock<ISqlServerRepository>();
			sqlRepo.Setup(r => r.AdminScriptsInstalled()).Returns(true);
			sqlRepo.Setup(sr => sr.EnvironmentCheckRepository.SaveServerDetails(It.IsAny<EnvironmentCheckServerDetails>()))
				.Callback<String, EnvironmentCheckServerDetails>((s, sd) =>
					Console.WriteLine(String.Format("Server Details: {0}, {1}, {2}, {3}, {4}", sd.ServerName, sd.Hyperthreaded, sd.LogicalProcessors, sd.OSName, sd.OSVersion))
				);

			//Act
			var tsk = new EnvironmentCheckServerInfoTask(logger.Object, sqlRepo.Object, 0);
			tsk.Execute(new ProcessControl());

			//Assert
			sqlRepo.Verify(sr => sr.EnvironmentCheckRepository.SaveServerDetails(It.Is<EnvironmentCheckServerDetails>(
				sd => sd.ServerName.ToLower() == Environment.MachineName.ToLower()
				&& sd.LogicalProcessors > 0
				&& false == String.IsNullOrEmpty(sd.OSName)
				&& false == String.IsNullOrEmpty(sd.OSVersion)
			)));
		}

		[Test]
		public void ExecuteEnvironmentCheckServerInfo_ExecuteTuningForkSystem()
		{
			//Arrange
			var logger = TestUtilities.GetMockLogger();

			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var sqlRepo = new SqlServerRepository(connectionFactory);

			//Act
			var tsk = new EnvironmentCheckServerInfoTask(logger.Object, sqlRepo, 0);
			tsk.Execute(new ProcessControl());

			//Assert

		}

		public string LocalIPAddress()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip.ToString();
					break;
				}
			}
			return localIP;
		}
	}
}
