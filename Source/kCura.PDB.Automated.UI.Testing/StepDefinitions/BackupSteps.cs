using kCura.PageModels.UI.Testing.PageModels.Relativity.PerformanceDashboard;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace kCura.PDB.Automated.UI.Testing.StepDefinitions
{
	[Binding]
	public class BackupSteps
	{
		[Then(@"I can see the number for Backups Overdue Databases is valid\.")]
		public void ThenICanSeeTheNumberForBackupsOverdueDatabasesIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(2, 1, 2), Is.True);
		}

		[Then(@"I can see the Backups Frequency Score is valid\.")]
		public void ThenICanSeeTheBackupsFrequencyScoreIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(2, 1, 3), Is.True);
		}

		[Then(@"I can see the Backups Coverage Score is valid\.")]
		public void ThenICanSeeTheBackupsCoverageScoreIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(2, 1, 4), Is.True);
		}
	}
}