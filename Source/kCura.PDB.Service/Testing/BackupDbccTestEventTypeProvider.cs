namespace kCura.PDB.Service.Testing
{
	using System.Collections.Generic;

	using kCura.PDB.Core.Interfaces.Testing;
	using kCura.PDB.Core.Models;

	public class BackupDbccTestEventTypeProvider : ITestEventTypeProvider
    {
        public IList<EventSourceType> GetEventTypesToEnqueue()
        {
            return new List<EventSourceType>
                       {
                           EventSourceType.CreateCategoryScoresForCategory,
                           EventSourceType.CreateMetricDatasForCategoryScores,
                           EventSourceType.CheckSamplingPeriodForMetricData,
                           EventSourceType.StartPrerequisitesForMetricData,
                           EventSourceType.CheckMetricDataIsReadyForDataCollection,
                           EventSourceType.CollectMetricData,
                           EventSourceType.ScoreMetricData,
                           EventSourceType.FindNextCategoriesToScore,
                           EventSourceType.ScoreCategoryScore,
                           EventSourceType.CompleteCategory
                       };
        }

        public EventSourceType GetEventTypeToComplete()
        {
            return EventSourceType.CompleteCategory;
        }
    }
}
