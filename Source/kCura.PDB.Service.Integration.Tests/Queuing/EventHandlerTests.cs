namespace kCura.PDB.Service.Integration.Tests.Queuing
{
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using Moq;
	using Ninject;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using EventHandler = kCura.PDB.Service.Queuing.EventHandler;
	using global::kCura.PDB.Tests.Common;
	using kCura.PDB.Core.Interfaces.Agent;
	using NUnit.Framework;
	
	[TestFixture]
	[Category("Integration")]
	public class EventHandlerTests
	{
		[Test]
		public async Task HandleEvent()
		{
			using (var kernel = GetKernel())
			{
				var eventRepository = kernel.Get<IEventRepository>();
				var eventHandler = kernel.Get<EventHandler>();
				var eventWorkerService = kernel.Get<IEventWorkerService>();

				//create a worker for our mock agent
				await eventWorkerService.CreateWorker();

				try
				{
					//create event
					var evnt = await eventRepository.CreateAsync(new Event
					{
						Status = EventStatus.PendingHangfire,
						SourceType = EventSourceType.NoOpSimple
					});

					await eventHandler.HandleEvent(evnt.Id, evnt.SourceType);

					//ASSERT
					evnt = await eventRepository.ReadAsync(evnt.Id);
					Assert.AreEqual(EventStatus.Completed, evnt.Status);
					Assert.AreEqual(null, evnt.Retries);
				}
				finally
				{
					//CLEANUP - restore original state
					await eventWorkerService.RemoveCurrentWorker();
				}
			}
		}

		[Test]
		public async Task HandleEvent_HandlesRetries()
		{
			using (var kernel = GetKernel())
			{
				var eventRepository = kernel.Get<IEventRepository>();
				var eventHandler = kernel.Get<EventHandler>();
				var eventWorkerService = kernel.Get<IEventWorkerService>();

				//create a worker for our mock agent
				await eventWorkerService.CreateWorker();

				try
				{
					//create event
					var evnt = await eventRepository.CreateAsync(new Event
					{
						Status = EventStatus.Pending,
						SourceType = EventSourceType.FailsThenSucceeds
					});


					foreach (var outcome in new[] 
					{
						Tuple.Create(EventStatus.Pending, 1),
						Tuple.Create(EventStatus.Pending, 2),
						Tuple.Create(EventStatus.Pending, 3),
						Tuple.Create(EventStatus.Error, 3) //don't retry again after three times
					})
					{
						//ARRANGE
						// make sure can be picked up by handler
						evnt = await eventRepository.ReadAsync(evnt.Id);
						evnt.Status = EventStatus.PendingHangfire;
						await eventRepository.UpdateAsync(evnt);

						//ACT
						await eventHandler.HandleEvent(evnt.Id, evnt.SourceType);

						//ASSERT
						evnt = await eventRepository.ReadAsync(evnt.Id);
						Assert.AreEqual(outcome.Item1, evnt.Status);
						Assert.AreEqual(outcome.Item2, evnt.Retries);
					};
				}
				finally
				{ 
					await eventWorkerService.RemoveCurrentWorker();
				}
			}
		}

		private static IKernel GetKernel()
		{
			var helper = TestUtilities.GetMockHelper(Config.RelativityServiceUrl, Config.RelativityRestUrl, Config.RSAPIUsername, Config.RSAPIPassword).Object;

			var agentService = Mock.Of<IAgentService>(x => x.AgentID == -1 && x.Name == $"{nameof(EventHandlerTests)} Mock");

			var kernel = IntegrationSetupFixture.CreateNewKernel();
			kernel.Rebind<global::Relativity.API.IHelper>().ToConstant(helper);
			kernel.Rebind<IAgentService>().ToConstant(agentService);
			return kernel;
		}
	}
}
