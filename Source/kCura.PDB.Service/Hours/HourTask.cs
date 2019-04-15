namespace kCura.PDB.Service.Hours
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Hours;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public class HourTask : IHourTask
	{
		public HourTask(IHourRepository hourRepository, IHourlyScoringLogic hourlyScoringLogic, IRatingsRepository ratingsRepository, IEventRepository eventRepository)
		{
			this.hourRepository = hourRepository;
			this.hourlyScoringLogic = hourlyScoringLogic;
			this.ratingsRepository = ratingsRepository;
			this.eventRepository = eventRepository;
		}

		private readonly IHourRepository hourRepository;
		private readonly IHourlyScoringLogic hourlyScoringLogic;
		private readonly IRatingsRepository ratingsRepository;
		private readonly IEventRepository eventRepository;

		public async Task<int> ScoreHour(int hourId)
		{
			await this.VerifyHourPrerequisitesMet(hourId);
			var hour = await this.hourRepository.ReadAsync(hourId);
			var result = await this.hourlyScoringLogic.ScoreHour(hour);
			hour.Score = result;
			await this.hourRepository.UpdateAsync(hour);
			return hourId;
		}

		public async Task<EventResult> CheckIfHourReadyToScore(int hourId)
		{
			// Check to see if we're already scoring the hour
			if (await this.eventRepository.ExistsAsync(hourId, (int)EventSourceType.ScoreHour))
			{
				return EventResult.Stop;
			}

			var hourReadyForScoring = await this.hourRepository.ReadHourReadyForScoringAsync(hourId);
			if (hourReadyForScoring != null)
			{
				// Make sure we meet the prerequisites for scoring the hour:
				await this.VerifyHourPrerequisitesMet(hourId);
				return new EventResult(hourId, EventSourceType.ScoreHour);
			}
			else
			{
				return EventResult.Stop;
			}
		}

		internal async Task VerifyHourPrerequisitesMet(int hourId)
		{
			// Check to see if ratings were already generated for this hour
			if (await this.ratingsRepository.Exists(hourId))
			{
				throw new Exception($"Ratings already created for given hour id {hourId}");
			}
		}
	}
}
