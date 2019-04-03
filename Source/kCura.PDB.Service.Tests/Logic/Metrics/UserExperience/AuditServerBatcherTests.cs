namespace kCura.PDB.Service.Tests.Logic.Metrics.UserExperience
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using kCura.PDB.Core.Interfaces.Audits;
    using kCura.PDB.Core.Interfaces.Metrics;
    using kCura.PDB.Core.Interfaces.Repositories;
    using kCura.PDB.Core.Interfaces.Services;
    using kCura.PDB.Core.Interfaces.Workspace;
    using kCura.PDB.Core.Models;
    using kCura.PDB.Core.Models.Audits;
    using kCura.PDB.Service.Audits;
    using kCura.PDB.Tests.Common;
    using Moq;
    using NUnit.Framework;

    [TestFixture, Category("Unit")]
    public class AuditServerBatcherTests
    {
        private AuditServerBatcher auditServerBatcher;
        private Mock<IAuditWorkspaceBatcher> auditWorkspaceBatcher;
        private Mock<IWorkspaceService> workspaceService;
        private Mock<ISearchAuditBatchRepository> searchAuditBatchRepository;
        private Mock<IMetricDataService> metricDataService;
        private ILogger logger;

        [SetUp]
        public void SetUp()
        {
            this.auditWorkspaceBatcher = new Mock<IAuditWorkspaceBatcher>();
            this.workspaceService = new Mock<IWorkspaceService>();
            this.searchAuditBatchRepository = new Mock<ISearchAuditBatchRepository>();
            this.metricDataService = new Mock<IMetricDataService>();
            this.logger = TestUtilities.GetMockLogger().Object;
            this.auditServerBatcher = new AuditServerBatcher(
                this.auditWorkspaceBatcher.Object,
                this.workspaceService.Object,
                this.searchAuditBatchRepository.Object,
                this.metricDataService.Object,
                this.logger);
        }

        [Test]
        public async Task CreateServerBatches()
        {
            // Arrange
            var metricDataId = 123456;
            var metricData = new MetricData
            {
                Id = metricDataId,
                Server = new Server
                {
                    ArtifactId = 123,
                    ServerId = 234
                },
                ServerId = 234,
                Metric = new Metric
                {
                    HourId = 456
                }
            };
            var workspaceBatches = new[]
            {
                new SearchAuditBatch(),
                new SearchAuditBatch(),
                new SearchAuditBatch { Id = 456, WorkspaceId = 123, BatchStart = 444, BatchSize = 5 } // should be filtered out for creating but not final list
			};
            var existingBatches = new[]
            {
                new SearchAuditBatch {Id = 456, WorkspaceId = 123, BatchStart = 444, BatchSize = 5},
            };
            var finalBatches = new[]
            {
                new SearchAuditBatch { Id = 123 },
                new SearchAuditBatch { Id = 234 },
                new SearchAuditBatch { Id = 345 },
                new SearchAuditBatch { Id = 456, WorkspaceId = 123, BatchStart = 444, BatchSize = 5 },
                new SearchAuditBatch { Id = 567 },
                new SearchAuditBatch { Id = 678 }
            };

            var workspaceIds = new[] { 444, 555 };

            var batchCountToCreate = workspaceBatches.Length * workspaceIds.Length - workspaceIds.Length;

            this.workspaceService.Setup(m => m.ReadAvailableWorkspaceIdsAsync(It.IsAny<int>()))
                .ReturnsAsync(workspaceIds);
            this.auditWorkspaceBatcher.Setup(m => m.CreateWorkspaceBatches(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(workspaceBatches);
            this.metricDataService.Setup(s => s.GetMetricData(metricDataId))
                .ReturnsAsync(metricData);
            this.searchAuditBatchRepository.Setup(r => r.CreateHourSearchAuditBatch(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(1));
            this.searchAuditBatchRepository.Setup(r => r.CreateBatches(It.Is<IList<SearchAuditBatch>>(list => list.Count == batchCountToCreate)))
                .Returns(Task.FromResult(1));
            this.searchAuditBatchRepository.SetupSequence(r => r.ReadBatchesByHourAndServer(metricData.Metric.HourId, metricData.ServerId.Value))
                .Returns(Task.FromResult(finalBatches.ToList() as IList<SearchAuditBatch>));
            this.searchAuditBatchRepository.Setup(m => m.DeleteAllBatchesAsync(It.IsAny<int>())).Returns(Task.Delay(1));


            // Act
            var result = await this.auditServerBatcher.CreateServerBatches(metricDataId);

            // Assert batch results
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(finalBatches.Length));
            this.searchAuditBatchRepository.Verify(r => r.CreateHourSearchAuditBatch(It.IsAny<int>(), It.IsAny<int>(), workspaceBatches.Length * workspaceIds.Length));
            this.searchAuditBatchRepository.Verify(r => r.CreateBatches(It.Is<IList<SearchAuditBatch>>(list => list.Count == finalBatches.Length)), "Verifies that an already existing record got filtered out");
            this.searchAuditBatchRepository.Verify(r => r.DeleteAllBatchesAsync(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void CreateServerBatches_ThrowException()
        {
            var metricDataId = 123456;
            var metricData = new MetricData { Server = null, ServerId = null };
            this.metricDataService.Setup(s => s.GetMetricData(metricDataId)).ReturnsAsync(metricData);

            Assert.ThrowsAsync<Exception>(() => this.auditServerBatcher.CreateServerBatches(metricDataId));
        }
    }
}
