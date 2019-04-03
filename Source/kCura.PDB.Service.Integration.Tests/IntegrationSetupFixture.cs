namespace kCura.PDB.Service.Integration.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlTypes;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;
	using DependencyInjection;
	using ILogger = kCura.PDB.Core.Interfaces.Services.ILogger;
	using Ninject.Modules;
	using kCura.PDB.Service.DataGrid;

	// [SetUpFixture]
	public class IntegrationSetupFixture
	{
		[OneTimeSetUp]
		public async Task Setup()
		{
			try
			{
				Logger = TestUtilities.GetMockLogger();
				Kernel = CreateNewKernel();

				this.metricService = Kernel.Get<IMetricService>();
				this.categoryService = Kernel.Get<ICategoryService>();

				if (!string.IsNullOrEmpty(Config.HourTimestamp))
				{
					var hourTimestamp = DateTime.Parse(Config.HourTimestamp).NormilizeToHour();
					Hour = await Repositories.HourRepository.CreateAsync(new Hour { HourTimeStamp = hourTimestamp });
				}
				else
				{
					Hour = await GetNextHour();
				}

				//var metricIdsTask = this.metricService.CreateMetricsForHour(Hour.Id);
				var categoryIds = await this.categoryService.CreateCategoriesForHour(Hour.Id);
				var categoryScoreIds = (await categoryIds.Select(cid => this.categoryService.CreateCategoryScoresForCategory(cid)).WhenAllStreamed()).SelectMany(csid => csid).ToList();
				var metricDataIds =
					(await categoryScoreIds.Select(csid => this.metricService.CreateMetricDatasForCategoryScores(csid)).WhenAllStreamed()).SelectMany(mdid => mdid).ToList();

				Categories =
					await categoryIds.Select(cid => Repositories.CategoryRepository.ReadAsync(cid)).WhenAllStreamed();
				MetricDatas =
					await metricDataIds.Select(mdid => Repositories.MetricDataRepository.ReadAsync(mdid)).WhenAllStreamed();
				CategoryScores =
					await categoryScoreIds.Select(csid => Repositories.CategoryScoreRepository.ReadAsync(csid)).WhenAllStreamed();
				Metrics =
					await MetricDatas.Select(md => md.MetricId).Distinct().Select(mid => Repositories.MetricRepository.ReadAsync(mid)).WhenAllStreamed();

				var servers = await Repositories.ServerRepository.ReadAllActiveAsync();
				var metricDataService = Kernel.Get<IMetricDataService>();
				var populatedMetricDatas = await MetricDatas.Select(async md => await metricDataService.GetMetricData(md.Id)).WhenAllStreamed();
				MetricDatas = populatedMetricDatas.ToList();
				Metrics.ForEach(m => m.Hour = Hour);
				CategoryScores
					.ForEach(cs => cs.Category = Categories.First(c => c.Id == cs.CategoryId))
					.Where(cs => cs.ServerId.HasValue)
					.ForEach(cs => cs.Server = servers.First(s => s.ServerId == cs.ServerId));
			}
			catch (Exception ex)
			{
				throw new Exception($"One time setup failed: {ex.ToString()}", ex);
			}
		}

		public static IKernel CreateNewKernel(bool includeDataGridBindings = false)
		{
			var bindings = includeDataGridBindings
				? new INinjectModule[] { new ServiceBindings(), new DataGridBindings() }
				: new INinjectModule[] { new ServiceBindings() };
			
			// Init Kernel
			Kernel = new KernelFactory(bindings).GetKernel(AgentConfiguration.DefaultBindingsExclusionList);
			
			// Init logger and bind to Kernel
			Logger = Logger ?? TestUtilities.GetMockLogger();
			Kernel.Bind<ILogger>().ToConstant(Logger.Object).InSingletonScope();

			// Init Metrics Repositories in the Kernel
			Repositories.Init(Kernel);

			// Return initialized Kernel
			return Kernel;
		}

		private async Task<Hour> GetNextHour()
		{
			var prevHighest = await Repositories.HourRepository.ReadHighestHourAfterMinHour();
			var nextHourTimeStamp = prevHighest != null
				? prevHighest.HourTimeStamp.NormilizeToHour().AddHours(1)
				: SqlDateTime.MinValue.Value.NormilizeToHour();
			return await Repositories.HourRepository.CreateAsync(new Hour { HourTimeStamp = nextHourTimeStamp });
		}

		private IMetricService metricService;
		private ICategoryService categoryService;

		public static IKernel Kernel;

		public static Mock<ILogger> Logger { get; private set; }

		public static Hour Hour { get; private set; }

		public static IList<Metric> Metrics { get; private set; }

		public static IList<MetricData> MetricDatas { get; private set; }

		public static IList<Category> Categories { get; private set; }

		public static IList<CategoryScore> CategoryScores { get; private set; }
	}
}
