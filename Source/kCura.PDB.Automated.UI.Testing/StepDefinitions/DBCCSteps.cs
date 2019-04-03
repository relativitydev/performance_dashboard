using kCura.PageModels.UI.Testing.PageModels.Relativity.PerformanceDashboard;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace kCura.PDB.Automated.UI.Testing.StepDefinitions
{
	[Binding]
	public class DBCCSteps
	{
		[Then(@"I can see the number for DBCC Overdue Databases is valid\.")]
		public void ThenICanSeeTheNumberForDBCCOverdueDatabasesIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(2, 2, 2), Is.True);
		}

		[Then(@"I can see the DBCC Frequency Score is valid\.")]
		public void ThenICanSeeTheDBCCFrequencyScoreIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(2, 2, 3), Is.True);
		}

		[Then(@"I can see the DBCC Coverage Score is valid\.")]
		public void ThenICanSeeTheDBCCCoverageScoreIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(2, 2, 4), Is.True);
		}
	}
}