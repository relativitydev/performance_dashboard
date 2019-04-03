namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Queuing;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class EventRunnerTests
	{
		[SetUp]
		public void Setup()
		{
			this.kernel = new StandardKernel();
		}

		private IKernel kernel;

		[Test]
		public async Task HandleEvent_Task()
		{
			// Arrange
			kernel.Bind<ITestEventTask>().ToConstant(new TestEventTask());

			// Act
			var runner = new EventRunner(this.kernel);
			var result = await runner.HandleEvent<ITestEventTask>(t => t.DoStuff(123), new Event());

			// Assert
			Assert.That(result.SourceIds, Is.Null);
		}

		[Test]
		public async Task HandleEvent_TaskInt()
		{
			// Arrange
			kernel.Bind<ITestEventTask>().ToConstant(new TestEventTask());

			// Act
			var runner = new EventRunner(this.kernel);
			var result = await runner.HandleEvent<ITestEventTask>(t => t.DoStuffInt(123), new Event());

			// Assert
			Assert.That(result.SourceIds.First(), Is.EqualTo(456));
		}

		[Test]
		public async Task HandleEvent_TaskListInt()
		{
			// Arrange
			kernel.Bind<ITestEventTask>().ToConstant(new TestEventTask());

			// Act
			var runner = new EventRunner(this.kernel);
			var result = await runner.HandleEvent<ITestEventTask>(t => t.DoStuffListInt(123), new Event());

			// Assert
			Assert.That(result.SourceIds.First(), Is.EqualTo(456));
		}

		[Test]
		public async Task HandleEvent_TaskListBool()
		{
			// Arrange
			kernel.Bind<ITestEventTask>().ToConstant(new TestEventTask());

			// Act
			var runner = new EventRunner(this.kernel);
			var result = await runner.HandleEvent<ITestEventTask>(t => t.DoStuffBool(123), new Event());

			// Assert
			Assert.That(result.Succeeded, Is.EqualTo(true));
		}

		[Test]
		public async Task HandleEvent_TaskEventResult()
		{
			// Arrange
			kernel.Bind<ITestEventTask>().ToConstant(new TestEventTask());

			// Act
			var runner = new EventRunner(this.kernel);
			var result = await runner.HandleEvent<ITestEventTask>(t => t.DoStuffEventResult(123), new Event());

			// Assert
			Assert.That(result.SourceIds.First(), Is.EqualTo(456));
		}

		[Test]
		public async Task HandleEvent_Action()
		{
			// Arrange
			kernel.Bind<ITestEventTask>().ToConstant(new TestEventTask());

			// Act
			var runner = new EventRunner(this.kernel);
			var result = await runner.HandleEvent<ITestEventTask>(t => t.DoStuff2(123), new Event());

			// Assert
			Assert.That(result.Succeeded, Is.True);
		}

		private interface ITestEventTask
		{
			Task DoStuff(int id);
			Task<int> DoStuffInt(int id);
			Task<bool> DoStuffBool(int id);
			Task<IList<int>> DoStuffListInt(int id);
			Task<EventResult> DoStuffEventResult(int id);
			void DoStuff2(int id);
		}

		public class TestEventTask : ITestEventTask
		{
			public Task DoStuff(int id)
			{
				return Task.Delay(10);
			}

			public Task<int> DoStuffInt(int id)
			{
				return Task.FromResult(456);
			}

			public Task<bool> DoStuffBool(int id)
			{
				return Task.FromResult(true);
			}

			public Task<IList<int>> DoStuffListInt(int id)
			{
				return Task.FromResult(new[] { 456, 567 } as IList<int>);
			}

			public Task<EventResult> DoStuffEventResult(int id)
			{
				return Task.FromResult(new EventResult(456));
			}

			public void DoStuff2(int id)
			{
				Console.WriteLine($"id: {id}");
			}
		}
	}
}
