namespace kCura.PDB.Service.Integration.Tests.SpecFlow.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
	using kCura.PDB.Agent.Bindings;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
    using kCura.PDB.Core.Interfaces.DatabaseDeployment;
    using kCura.PDB.Core.Interfaces.Repositories;
    using kCura.PDB.Core.Models.BISSummary.Grids;
    using kCura.PDB.Data.Repositories.BISSummary;
    using kCura.PDB.Data.Testing;
    using kCura.PDB.DependencyInjection;
    using kCura.PDB.Service.Bindings;
    using kCura.PDB.Service.BISSummary;
    using kCura.PDB.Service.Integration.Tests.Bindings;
    using kCura.PDB.Service.Integration.Tests.Properties;
    using kCura.PDB.Service.Testing;
    using kCura.PDB.Tests.Common;

    using Moq;

    using Ninject;
    using Ninject.Modules;

    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class BackupDbccSteps
    {
        private Mock<IProcessControlRepository> processControlRepositoryMock;

        [BeforeScenario]
        public void Setup()
        {
	        var integrationHelper = TestUtilities.GetMockHelper();
			var bindings = new List<INinjectModule> { new TestExecutionBindings(), new AgentIntegrationTestBindings(), new RelativityBindings(integrationHelper.Object) };
            var kernel = KernelFactory.GetKernel(bindings);
			this.SetupTestingSystem(kernel);
			ScenarioContext.Current["Kernel"] = kernel;
        }

        [Given(@"I import a mock data set that should return RPO score of (.*)")]
        public async Task GivenIImportAMockDataSetThatShouldReturnRPOScoreOf(int p0)
        {
            var kernel = ScenarioContext.Current.Get<IKernel>("Kernel");

            // Import excel sheet using tool/service
            var testDataService = kernel.Get<TestDataSetupService>();
            var testData = await testDataService.SetupBackupDbccDataAsync(Resources.RPO_100_score);
            ScenarioContext.Current["Hours"] = testData.Hours.Select(h => h.HourTimeStamp).ToList();
        }

        [Given(@"I clean BackupDbcc report data for the given hours")]
        public async Task GivenICleanBackupDbccReportDataForTheGivenHours()
        {
            // Get Datetime hours from context?
            var hours = ScenarioContext.Current.Get<IList<DateTime>>("Hours");
            var kernel = ScenarioContext.Current.Get<IKernel>("Kernel");

            // Cleanup the report data
            var reportCleanupService = kernel.Get<ReportCleanupService>();
            await reportCleanupService.ClearReportDataAsync(hours);
        }

        [When(@"I execute the BackupDBCCService on the mock data")]
        public async Task WhenIExecuteTheBackupDBCCServiceOnTheMockData()
        {
            var kernel = ScenarioContext.Current.Get<IKernel>("Kernel");
			var execution = kernel.Get<OldBackupDbccServiceExecution>();
            await execution.ExecuteAsync(CancellationToken.None);
        }

	    internal void SetupTestingSystem(IKernel kernel)
	    {
		    var primarySqlServerRepository = kernel.Get<IPrimarySqlServerRepository>();
		    var databaseMigratorFactory = kernel.Get<IDatabaseMigratorFactory>();

		    // Deploy the mock sproc used for testing the old system
		    var primaryServer = primarySqlServerRepository.GetPrimarySqlServer();
		    var migrator = databaseMigratorFactory.GetTestingDeploymentMigrator(
			    primaryServer.ServerInstance,
			    Names.Database.EddsPerformance,
			    Resources.MigrateTesting);

		    var deployResult = migrator.Deploy();
		    if (!deployResult.Success)
		    {
			    throw new Exception($"Failed to install scripts, details: {string.Join("\r\n| ", deployResult.Messages.Select(m => m.Message))}");
		    }

		    // Just in case, force redeploy of the sprocs
		    var redeployResult = migrator.ReDeployScripts();
		    if (!redeployResult.Success)
		    {
			    throw new Exception($"Failed to install scripts, details: {string.Join("\r\n| ", redeployResult.Messages.Select(m => m.Message))}");
		    }
	    }

		[When(@"the event system scores the Recoverability/Integrity category scored for the given hours")]
        public async Task WhenTheEventSystemScoresTheRecoverabilityIntegrityCategoryScoredForTheGivenHours()
        {
            var kernel = ScenarioContext.Current.Get<IKernel>("Kernel");
            var execution = kernel.Get<NewBackupDbccServiceExecution>();
            await execution.ExecuteAsync(CancellationToken.None);
        }


        [Then(@"the RPO score for the hour should be (.*)")]
        public async Task ThenTheRPOScoreForTheHourShouldBe(int p0)
        {
            // Build up service (api) to get report data
            var kernel = ScenarioContext.Current.Get<IKernel>("Kernel");
	        kernel.Bind<IRecoverabilityIntegrityReportReader>().To<LegacyRecoverabilityIntegrityReportRepository>();
            var backupDbccService = kernel.Get<BackupDbccService>();

	        // Read test hours
	        var mockHourRepository = kernel.Get<HourTestDataRepository>();
	        var hours = await mockHourRepository.ReadHoursAsync();

			// Init params
	        var startDate = hours.Min(h => h.HourTimeStamp);
	        var gridConditions = new GridConditions { StartDate = startDate, EndDate = startDate };

			// Get result
	        var result = backupDbccService.RecoverabilityIntegritySummary(
		        gridConditions,
		        new RecoverabilityIntegrityViewFilterConditions(),
		        new RecoverabilityIntegrityViewFilterOperands());
            
            // Should only be one result, unless we're running multiple hours
            var filteredResults = result.Data.Where(d => hours.Any(h => h.HourTimeStamp == d.SummaryDayHour));
            var orderedResults = filteredResults.OrderByDescending(o => o.Index);
            var firstResult = orderedResults.First();

            // Assert TODO -- Discuss if assertions should be in the excel sheet in some format, or how they should work.
            Assert.That(firstResult.RPOScore, Is.EqualTo(p0));
        }

	    [Then(@"the RPO category score for the hour should be (.*)")]
	    public async Task ThenTheRPOCategoryScoreForTheHourShouldBe(int p0)
	    {
		    var kernel = ScenarioContext.Current.Get<IKernel>("Kernel");
		    var hourTestRepo = kernel.Get<HourTestDataRepository>();
		    var hours = await hourTestRepo.ReadHoursAsync();

		    var categoryScoreRepo = kernel.Get<ICategoryScoreRepository>();
		    var hoursCategoryScores = await hours.Select(h => categoryScoreRepo.ReadAsync(h)).WhenAllStreamed();

			Assert.That(hoursCategoryScores, Is.Not.Empty);
		    hoursCategoryScores.ForEach(hcs => hcs.ForEach(s => Assert.That(s.Score, Is.EqualTo(p0))));
	    }

	}
}
