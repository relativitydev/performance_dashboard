namespace kCura.PDB.Service.Tests.ProcessControls.HealthPerformance
{
	using System;
	using System.Reflection;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.HealthPerformance;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit"), Ignore("TODO fix this so that internals can be called and it runs in short amount of time")]
	public class RamHealthPerformanceTaskTests
	{
		[Test, Explicit("TODO Investigate why this runs too long for TeamCity builds.")]
		public void ProcessServer()
		{
			//Arrange
			var pass_or_fail = false;
			var server = new Server
			{
				ServerId = 1,
				ServerIpAddress = "127.0.0.1",
				ServerName = "localhost"
			};
			var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			var minfo = typeof(RamHealthPerformanceTask).GetMethod("ProcessServer", bindingFlags);
			var logger = new Mock<ILogger>();

			var parms = new object[] { server };
			var task = new RamHealthPerformanceTask() { Logger = logger.Object };

			//Act
			try
			{
				//invoke instance via reflection b/c internal
				minfo.Invoke(task, parms);
				pass_or_fail = true;
			}
			catch (Exception) { }

			//Assert
			Assert.IsTrue(pass_or_fail);
		}
	}
}
