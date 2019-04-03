using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.PDD.Web.Services;
using kCura.Relativity.Client;
using Moq;
using NUnit.Framework;
using global::Relativity.API;

namespace kCura.PDD.Web.Test.Services
{
	[TestFixture, Category("Unit")]
	public class ErrorHandlingServiceTests
	{
		[Test]
		public void LogToErrorLog()
		{
			//Arrange
			var helper = new Mock<IHelper>();
			helper.Setup(h => h.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				.Returns(new Mock<IRSAPIClient>().Object);

			//Act
			var srv = new ErrorHandlingService(helper.Object);
			var result = srv.LogToErrorLog(new Exception("Problems"), "http://localhost/relativity");

			//Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void LogToErrorLog_HandlesError()
		{
			//Arrange
			var helper = new Mock<IHelper>();
			helper.Setup(h => h.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				.Throws(new Exception("error"));

			//Act
			var srv = new ErrorHandlingService(helper.Object);
			var result = srv.LogToErrorLog(new Exception("Problems"), "http://localhost/relativity");

			//Assert
			Assert.That(result, Is.False);
		}

		[Test, Ignore]
		public void LogToEventViewer()
		{
			//Act
			var srv = new ErrorHandlingService();
			var result = srv.LogToEventViewer(new Exception("Problems"), "http://localhost/relativity", "");

			//Assert
			Assert.That(result, Is.True);
		}
	}
}
