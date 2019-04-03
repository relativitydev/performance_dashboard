using Dapper;
using kCura.PDB.Automated.UI.Testing.Constants;
using kCura.PDB.Tests.Common;
using Milyli.UIAutomation.Relativity.Utils;
using NUnit.Framework;
using System.Data.SqlClient;
using kCura.PageModels.UI.Testing.PageModels.Relativity.PerformanceDashboard;
using kCura.PDB.Automated.UI.Testing.PageModels.Relativity.PerformanceDashboard;
using kCura.PDB.Automated.UI.Testing.StepDefinitions;
using Milyli.UIAutomation.Relativity.PageModels;
using TechTalk.SpecFlow;

namespace kCura.PDB.Automated.UI.Testing.Gherkin
{
	[Binding]
	public class PDBAutomatedUITests
	{
		// For additional details on SpecFlow step definitions see http://go.specflow.org/doc-stepdef

		// Given 

		[Given(@"that I am on the Reinstall Scripts page\.")]
		public void GivenThatIAmOnTheReinstallScriptsPage_()
		{
			var relativityHomePageModel = new RelativityHomePageModel(Configuration.Driver);
			relativityHomePageModel.ClickTab(PageConstants.RecoverabilityIntegrityTab);
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			recoverabilityIntegrityPage.ReInstallScriptsSubTab.Click();
		}

		[Given(@"I enter the valid SQL credentials on the Reinstall Scripts page\.")]
		public void GivenIEnterTheAndOnTheReinstallScriptsPage_()
		{
			var installSQLScriptsPage = new InstallSQLScriptsPage();
			installSQLScriptsPage.UsernameInput.SendKeys(Config.SAUserName);
			installSQLScriptsPage.UsernamePassword.SendKeys(Config.SAPassword);
		}

		[Given(@"that I generate an Errored Event\.")]
		public void GivenThatIGenerateAnErroredEvent_()
		{
			var erroredEventScript = Properties.Resources.createsuccessfuldummyevent;
			using (var connection = new SqlConnection(Config.PrimaryConnectionString))
			{
				connection.Execute(erroredEventScript, new { status = 4 });
			}
		}

		[Given(@"that I am on the Backfill Console page\.")]
		public void GivenThatIAmOnTheBackfillConsolePage_()
		{
			var relativityHomePageModel = new RelativityHomePageModel(Configuration.Driver);
			relativityHomePageModel.ClickTab(PageConstants.BackfillConsoleTab);
		}

		// When

		[When(@"I run the SQL Script Install\.")]
		public void WhenIRunTheSQLScriptInstall_()
		{
			var installSQLScriptsPage = new InstallSQLScriptsPage();
			installSQLScriptsPage.RunButton.Click();
		}

		[When(@"I navigate to the '(.*)' page\.")]
		public void WhenINavigateToThePage_(string targetPage)
		{
			var relativityHomePageModel = new RelativityHomePageModel(Configuration.Driver);
			relativityHomePageModel.ClickTab(targetPage);
		}

		[When(@"I press the Retry Errored Events button\.")]
		public void WhenIPressTheRetryErroredEventsButton_()
		{
			var backfillConsolePage = new BackfillConsolePage(Configuration.Driver);
			backfillConsolePage.RetryErroredEventsButton.Click();
		}

		[When(@"there are no errored events remaining\.")]
		public void WhenThereAreNoErroredEventsRemaining_()
		{
			var backfillConsolePage = new BackfillConsolePage(Configuration.Driver);
			Assert.That(int.Parse(backfillConsolePage.ErroredEventsCount.Text) == 0, Is.True);
		}

		[When(@"I have navigated to the ""(.*)"" page\.")]
		public void WhenIHaveNavigatedToThePage_(string page)
		{
			var relativityHomePageModel = new RelativityHomePageModel(Configuration.Driver);
			relativityHomePageModel.ClickTab(page);
		}

		[When(@"I have navigated to the Backup/DBCC Report subpage\.")]
		public void WhenIHaveNavigatedToTheBackupDBCCReportSubpage_()
		{
			var relativityHomePageModel = new RelativityHomePageModel(Configuration.Driver);
			relativityHomePageModel.ClickTab(PageConstants.RecoverabilityIntegrityTab);
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			recoverabilityIntegrityPage.BackupDBCCReportSubTab.Click();
		}

		[When(@"I have navigated to the Recovery Objectives Report subpage\.")]
		public void WhenIHaveNavigatedToTheRecoveryObjectivesReportSubpage_()
		{
			var relativityHomePageModel = new RelativityHomePageModel(Configuration.Driver);
			relativityHomePageModel.ClickTab(PageConstants.RecoverabilityIntegrityTab);
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			recoverabilityIntegrityPage.RecoveryObjectivesReportSubTab.Click();
		}

		// Then

		[Then(@"I receive SQL Script Install success message\.")]
		public void ThenIReceiveSQLScriptInstallSuccessMessage_()
		{
			var installSQLScriptsPage = new InstallSQLScriptsPage();
			PageConstants.WaitUntil(() => installSQLScriptsPage.SuccessMessageWindow.Displayed, 60);
		}

		[Then(@"the page '(.*)' loads\.")]
		public void ThenThePageLoads_(string resultPage)
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			switch (resultPage)
			{
				case PageConstants.QualityOfServiceTab:
					qualityOfServicePage.VerifyQoSPageNavigation();
					break;
				default:
					Assert.Fail($"Unexpected page: {resultPage}");
					break;
			}
		}

		[Then(@"the Retry Errored Events button is not present\.")]
		public void ThenTheRetryErroredEventsButtonIsNotPresent_()
		{
			var backfillConsolePage = new BackfillConsolePage(Configuration.Driver);
			Assert.That(backfillConsolePage.VerifyRetryErroredEventsButtonIsNotPresent(), Is.True);
		}

		[Then(@"there are no errored events remaining\.")]
		public void ThenThereAreNoErroredEventsRemaining_()
		{
			var backfillConsolePage = new BackfillConsolePage(Configuration.Driver);
			Assert.That(int.Parse(backfillConsolePage.ErroredEventsCount.Text) == 0, Is.True);
		}

		[Then(@"the '(.*)' Page with the score loads\.")]
		public void ThenThePageWithTheScoreLoads_(string scoringPage)
		{
			Assert.That(NavigationVerification.VerifyPageNavigation(scoringPage, new BasePage(Configuration.Driver)), $"{scoringPage} page was not loaded", Is.True);
		}

		[Then(@"I see Quaterly RI Score exist on the page\.")]
		public void ThenISeeQuaterlyRIScoreExistOnThePage_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.RIQuaterlyScoreSummaryPeport.Displayed, Is.True);
		}

		[Then(@"I see Weekly RI Score exist on the page\.")]
		public void ThenISeeWeeklyRIScoreExistOnThePage_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.RIWeeklyScoreSummaryPeport.Displayed, Is.True);
		}

		[Then(@"I can see the Max Data Loss Time is valid\.")]
		public void ThenICanSeeTheMaxDataLossTimeIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedTimeValue(1), Is.True);
		}

		[Then(@"I can see the Time To Recover Time is valid\.")]
		public void ThenICanSeeTheTimeToRecoverTimeIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedTimeValue(2), Is.True);
		}

		[Then(@"I can see the Max Data Loss Score is valid\.")]
		public void ThenICanSeeTheMaxDataLossScoreIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(3, 1, 3), Is.True);
		}

		[Then(@"I can see the Time to Recover Score is valid\.")]
		public void ThenICanSeeTheTimeToRecoverScoreIsValid_()
		{
			var qualityOfServicePage = new QualityOfServicePage(Configuration.Driver);
			Assert.That(qualityOfServicePage.VerifyExpectedScoreNumberValue(3, 2, 3), Is.True);
		}

		[Then(@"I can see the RI Score shows up on the page\.")]
		public void ThenICanSeeTheRIScoreShowsUpOnThePage_()
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.VerifyRIScoreShowsUpOnThePage(), Is.True);
		}

		[Then(@"the user can see the list is pagable\.")]
		public void ThenTheUserCanSeeTheListIsPagable_()
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.VerifyListIsPagable(), Is.True);
		}

		[Then(@"the user can see the list is sortable for all columns\.")]
		public void ThenTheUserCanSeeTheListIsSortableForAllColumns_()
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.VerifyListIsSortable(), Is.True);
		}

		[Then(@"the user can see the list is filtarable for all columns\.")]
		public void ThenTheUserCanSeeTheListIsFiltarableForAllColumns_()
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.VerifyListIsFiltarable("0"), Is.True);
		}

		[Then(@"I can see the Quarterly RI Score is shown on the page\.")]
		public void ThenICanSeeTheQuarterlyRIScoreIsShownOnThePage_()
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.VerifyQuaterlyScoreIsShownInTheTopLeft(), Is.True);
		}

		[Then(@"I can see the Chart is present on the page\.")]
		public void ThenICanSeeTheChartIsPresentOnThePage_()
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.Chart.Displayed, Is.True);
		}

		[Then(@"the user can see the score of '(.*)' is coded in '(.*)' color\.")]
		public void ThenTheUserCanSeeTheScoreOfIsCodedInColor_(int scoreNumber, string scoreColor)
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			Assert.That(recoverabilityIntegrityPage.VerifyExpectedNumberValue(scoreNumber), Is.True);
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			var colorText = backupDbccReportPagePage.scoreColor[scoreColor.ToLower()];
			Assert.That(recoverabilityIntegrityPage.VerifyExpectedColor(colorText), Is.True);
		}

		[Then(@"the user can see the '(.*)' list is exportable on the Recoverability/Integrity page and the file exists at the '(.*)'\.")]
		public void ThenTheUserCanSeeTheListIsExportableOnTheRecoverabilityIntegrityPageAndTheFileExistsAtThe_(string fileName, string filePath)
		{
			var recoverabilityIntegrityPage = new RecoverabilityIntegrityPage(Configuration.Driver);
			recoverabilityIntegrityPage.VerifyListIsExportable(fileName, Config.DownloadPath);
		}

		[Then(@"I can see the Backup/DBCC report data shows up on the page\.")]
		public void ThenICanSeeTheBackupDBCCReportDataShowsUpOnThePage_()
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyBackupDBCCDataShowsUpOnThePage, Is.True);
		}

		[Then(@"I can see the Quarterly RI Score is shown on the Backup/DBCC Report page\.")]
		public void ThenICanSeeTheQuarterlyRIScoreIsShownOnTheBackupDBCCReportPage_()
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyQuaterlyScoreIsShownInTheTopLeft, Is.True);
		}

		[Then(@"I can see the Chart is present on the Backup/DBCC Report page\.")]
		public void ThenICanSeeTheChartIsPresentOnTheBackupDBCCReportPage_()
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.Chart.Displayed, Is.True);
		}

		[Then(@"the user can see the list is sortable for all columns on the Backup/DBCC Report page\.")]
		public void ThenTheUserCanSeeTheListIsSortableForAllColumnsOnTheBackupDBCCReportPage_()
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyListIsSortable(), Is.True);
		}

		[Then(@"the user can see the list is pagable on the Backup/DBCC Report page\.")]
		public void ThenTheUserCanSeeTheListIsPagableOnTheBackupDBCCReportPage_()
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyListIsPagable, Is.True);
		}

		[Then(@"the user can see the list is filtarable for all columns on the Backup/DBCC Report page\.")]
		public void ThenTheUserCanSeeTheListIsFiltarableForAllColumnsOnTheBackupDBCCReportPage_()
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyListIsFiltarable("397"), Is.True);
		}

		[Then(@"the user can see the Gap size of '(.*)' days is coded in '(.*)' color\.")]
		public void ThenTheUserCanSeeTheGapSizeOfDaysIsCodedInColor_(int gapSize, string gapSizeColor)
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyExpectedNumberValue(gapSize), Is.True);
			var colorText = backupDbccReportPagePage.gapSizeColor[gapSizeColor.ToLower()];
			Assert.That(backupDbccReportPagePage.VerifyExpectedColor(colorText), Is.True);
		}

		[Then(@"the user can see the Gap size of '(.*)' days with Gap Resolution Date '(.*)' is coded in '(.*)' color\.")]
		public void ThenTheUserCanSeeTheGapSizeOfDaysWithGapResolutionDateIsCodedInColor_(int gapSize, string gapResolutionDate, string gapSizeColor)
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyExpectedNumberValue(gapSize), Is.True);
			Assert.That(backupDbccReportPagePage.VerifyGapResolutionDate(gapResolutionDate), Is.True);
			var colorText = backupDbccReportPagePage.gapSizeColor[gapSizeColor.ToLower()];
			Assert.That(backupDbccReportPagePage.VerifyExpectedColor(colorText), Is.True);
		}

		[Then(@"the user can see the Gap size of '(.*)' days with '(.*)' is coded in '(.*)' color\.")]
		public void ThenTheUserCanSeeTheGapSizeOfDaysWithIsCodedInColor_(int gapSize, string gapResolutionDate, string gapSizeColor)
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			Assert.That(backupDbccReportPagePage.VerifyExpectedNumberValue(gapSize), Is.True);
			Assert.That(backupDbccReportPagePage.VerifyGapResolutionDate(gapResolutionDate), Is.True);
			var colorText = backupDbccReportPagePage.gapSizeColor[gapSizeColor.ToLower()];
			Assert.That(backupDbccReportPagePage.VerifyExpectedColor(colorText), Is.True);
		}

		[Then(@"the user can see the '(.*)' list is exportable on the Backup/DBCC Report page and the file exists at the '(.*)'\.")]
		public void ThenTheUserCanSeeTheListIsExportableOnTheBackupDBCCReportPageAndTheFileExistsAtThe_(string fileName, string filePath)
		{
			var backupDbccReportPagePage = new BackupDBCCReportPage(Configuration.Driver);
			backupDbccReportPagePage.VerifyListIsExportable(fileName, Config.DownloadPath);
		}

		[Then(@"I can see the Chart is present on the Recovery Objectives Report page\.")]
		public void ThenICanSeeTheChartIsPresentOnTheRecoveryObjectivesReportPage_()
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.Chart.Displayed, Is.True);
		}

		[Then(@"I can see the Recovery Objectives report data shows up on the page\.")]
		public void ThenICanSeeTheRecoveryObjectivesReportDataShowsUpOnThePage_()
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.VerifyRecoveryObjectivesReportDataShowsUpOnThePage, Is.True);
		}

		[Then(@"I can see the Quarterly RI Score is shown on the Recovery Objectives Report page\.")]
		public void ThenICanSeeTheQuarterlyRIScoreIsShownOnTheRecoveryObjectivesReportPage_()
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.VerifyQuaterlyScoreIsShownInTheTopLeft, Is.True);
		}

		[Then(@"the user can see the list is pagable on the Recovery Objectives Report page\.")]
		public void ThenTheUserCanSeeTheListIsPagableOnTheRecoveryObjectivesReportPage_()
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.VerifyListIsPagable, Is.True);
		}

		[Then(@"the user can see the list is filtarable for all columns on the Recovery Objectives Report page\.")]
		public void ThenTheUserCanSeeTheListIsFiltarableForAllColumnsOnTheRecoveryObjectivesReportPage_()
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.VerifyListIsFiltarable("60"), Is.True);
		}
		[Then(@"the user can see the list is sortable for all columns on the Recovery Objectives Report page\.")]
		public void ThenTheUserCanSeeTheListIsSortableForAllColumnsOnTheRecoveryObjectivesReportPage_()
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.VerifyListIsSortable(), Is.True);
		}

		[Then(@"the user can see the score of '(.*)' is coded in '(.*)' color on the Recovery Objectives Report page\.")]
		public void ThenTheUserCanSeeTheScoreOfIsCodedInColorOnTheRecoveryObjectivesReportPage_(int scoreNumber, string scoreColor)
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			Assert.That(recoveryObjectivesReportPage.VerifyExpectedNumberValue(scoreNumber), Is.True);
			var colorText = recoveryObjectivesReportPage.scoreColor[scoreColor.ToLower()];
			Assert.That(recoveryObjectivesReportPage.VerifyExpectedColor(colorText), Is.True);
		}

		[Then(@"the user can see the '(.*)' list is exportable on the Recovery Objectives Report page and the file exists at the '(.*)'\.")]
		public void ThenTheUserCanSeeTheListIsExportableOnTheRecoveryObjectivesReportPageAndTheFileExistsAtThe_(string fileName, string filePath)
		{
			var recoveryObjectivesReportPage = new RecoveryObjectivesReportPage(Configuration.Driver);
			recoveryObjectivesReportPage.VerifyListIsExportable(fileName, Config.DownloadPath);
		}
	}
}