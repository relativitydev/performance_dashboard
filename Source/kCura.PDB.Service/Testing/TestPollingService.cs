namespace kCura.PDB.Service.Testing
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Testing;
	using kCura.PDB.Core.Interfaces.Testing.Repositories;
	using kCura.PDB.Core.Interfaces.Testing.Services;
	using kCura.PDB.Core.Models;

	public class TestPollingService : ITestPollingService
	{
		private readonly IEventRepository eventRepository;
		private readonly IHourTestDataRepository hourTestDataRepository;
		private readonly ITestEventTypeProvider testEventTypeProvider;

		public TestPollingService(
			IEventRepository eventRepository,
			IHourTestDataRepository hourTestDataRepository,
			ITestEventTypeProvider testEventTypeProvider)
		{
			this.eventRepository = eventRepository;
			this.hourTestDataRepository = hourTestDataRepository;
			this.testEventTypeProvider = testEventTypeProvider;
		}

		public async Task WaitUntilEventCompletionAsync(CancellationToken cancellationToken)
		{
			var pollingDetails = await this.GetMetricEventPollingDetailsAsync();
			await this.WaitUntilEventFoundAsync(
				pollingDetails.FinalHourId,
				pollingDetails.FinalEventType,
				cancellationToken);
		}

		private async Task WaitUntilEventFoundAsync(int hourId, EventSourceType eventSourceType, CancellationToken cancellationToken)
		{
			var eventFound = false;
			while (!eventFound && !cancellationToken.IsCancellationRequested)
			{
				await Task.Delay(500, cancellationToken);
				eventFound = await this.eventRepository.ExistsAsync(hourId, eventSourceType, EventStatus.Completed);
			}
		}

		private async Task<MetricEventPollingDetails> GetMetricEventPollingDetailsAsync()
		{
			// Retrieve the needed data for discerning the lifetime of the manager
			var eventToWatch = this.testEventTypeProvider.GetEventTypeToComplete();
			var hours = await this.hourTestDataRepository.ReadHoursAsync();
			var hourToWatch = hours.OrderBy(h => h.HourTimeStamp).First();
			return new MetricEventPollingDetails { FinalEventType = eventToWatch, FinalHourId = hourToWatch.Id };
		}

		private class MetricEventPollingDetails
		{
			public int FinalHourId { get; set; }
			public EventSourceType FinalEventType { get; set; }
		}
	}
}
