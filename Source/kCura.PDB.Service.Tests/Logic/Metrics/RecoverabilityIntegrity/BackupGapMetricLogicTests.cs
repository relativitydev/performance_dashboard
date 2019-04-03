namespace kCura.PDB.Service.Tests.Logic.Metrics.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.RecoverabilityIntegrity;
	using kCura.PDB.Service.Metrics.RecoverabilityIntegrity;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	[Category("Unit")]
	public class BackupGapMetricLogicTests
	{
		private BackupGapMetricLogic backupGapMetricLogic;
		private Mock<IGapsCollectionVerifier> gapsCollectionVerifier;

		[SetUp]
		public void Setup()
		{
			this.gapsCollectionVerifier = new Mock<IGapsCollectionVerifier>();
			this.backupGapMetricLogic = new BackupGapMetricLogic(this.gapsCollectionVerifier.Object);
		}
	}
}
