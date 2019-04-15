namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class EventTaskFactoryTests
	{
		[SetUp]
		public void Setup()
		{
			this.logger = TestUtilities.GetMockLogger();
			this.eventTaskFactory = new EventTaskFactory();
		}

		private Mock<ILogger> logger;
		private EventTaskFactory eventTaskFactory;

		[Test,
			TestCase(EventSourceType.CreateNextHour, typeof(EventTask)),
			TestCase(EventSourceType.CheckMetricDataIsReadyForDataCollection, typeof(CheckEventTask))]
		public void GetEventTask(EventSourceType eventType, Type expectedType)
		{
			// Arrange
			using (var kernel = new StandardKernel())
			{
				kernel.Bind<EventTask>().ToMethod(c => new EventTask(null, null, this.logger.Object, null));
				kernel.Bind<CheckEventTask>().ToMethod(c => new CheckEventTask(null, null, this.logger.Object, null));

				// Act
				var result = this.eventTaskFactory.GetEventTask(kernel, eventType);

				// Assert
				Assert.That(result.GetType(), Is.EqualTo(expectedType));
			}
		}
	}
}

