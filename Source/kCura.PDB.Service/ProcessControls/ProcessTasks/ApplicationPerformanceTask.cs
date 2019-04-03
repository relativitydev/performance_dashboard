namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data;

	[Description("Application Performance")]
	public class ApplicationPerformanceTask : BaseProcessControlTask, IProcessControlTask
	{
		public ApplicationPerformanceTask(ILogger logger, ISqlServerRepository sqlRepo, int agentId)
			: base(logger, sqlRepo, agentId)
		{
			this.logger = logger.WithClassName();
			this.sqlRepo = sqlRepo;
		}

		public ApplicationPerformanceTask(ILogger logger, ISqlServerRepository sqlRepo, IPDDModelDataContext pddModelDataContext, int agentId)
			: base(logger, sqlRepo, agentId)
		{
			this.logger = logger.WithClassName();
			this.sqlRepo = sqlRepo;
			this.pddModelDataContext = pddModelDataContext;
		}

		private readonly ILogger logger;
		private readonly ISqlServerRepository sqlRepo;
		private readonly IPDDModelDataContext pddModelDataContext;
		public enum ProcTypes
		{
			LoadErrorHealthDwData = 2,
			LoadUserHealthDwData = 4
		}

		public ProcessControlId ProcessControlID => ProcessControlId.ApplicationPerformance;

		public bool Execute(ProcessControl processControl)
		{
			logger.LogVerbose("GetPerformanceMetrics Called");

			try
			{
				//2	Errors
				logger.LogVerbose("Calling LoadErrorHealthDWData");
				LoadApplicationDw(ProcTypes.LoadErrorHealthDwData);
				logger.LogVerbose("Calling LoadErrorHealthDWData - Done");

				//4	Users
				logger.LogVerbose("Calling LoadUserHealthDWData");
				LoadApplicationDw(ProcTypes.LoadUserHealthDwData);
				logger.LogVerbose("Calling LoadUserHealthDWData - Done");

				logger.LogVerbose("GetPerformanceMetrics Called - Success");
			}
			catch (Exception ex)
			{
				logger.LogError($"GetPerformanceMetrics: {ex.Message}", ex);
			}

			return true;
		}

		public void LoadApplicationDw(ProcTypes procType)
		{
			logger.LogVerbose("LoadApplicationDW Called");

			try
			{
				using (var dataContext = this.pddModelDataContext ?? new PDDModelDataContext())
				{
					var measure = dataContext.ReadMeasures().FirstOrDefault(m => m.MeasureID == (int)procType);

					// Process only if the frequency is greater than 0
					if (measure.Frequency > 0)
					{
						//If the Last Data value is null, look back 7 days from now, so that data can be back-filled
						if (measure.LastDataLoadDateTime == null)
						{
							measure.LastDataLoadDateTime = measure.LastDataLoadDateTime.GetValueOrDefault(DateTime.UtcNow.AddDays(Defaults.BackfillDays - 1));
						}

						while (measure.LastDataLoadDateTime.Value <= DateTime.UtcNow)
						{
							switch (procType)
							{
								case ProcTypes.LoadErrorHealthDwData:
									logger.LogVerbose($"LoadErrorHealthDWData for {measure.LastDataLoadDateTime}",
										this.GetType().Name);
									sqlRepo.PerformanceSummaryRepository.LoadErrorHealthDwData(measure.LastDataLoadDateTime.Value);
									break;
								case ProcTypes.LoadUserHealthDwData:
									logger.LogVerbose($"LoadUserHealthDWData for {measure.LastDataLoadDateTime}",
										this.GetType().Name);
									sqlRepo.PerformanceSummaryRepository.LoadUserHealthDwData(measure.LastDataLoadDateTime.Value);
									break;
							}

							measure.LastDataLoadDateTime = measure.LastDataLoadDateTime.Value.AddMinutes((double)measure.Frequency);
							dataContext.SubmitChanges();
						}
					}
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				logger.LogError($"LoadApplicationDW: {message}");
			}

			logger.LogVerbose("LoadApplicationDW Called - Success");
		}

	}
}
