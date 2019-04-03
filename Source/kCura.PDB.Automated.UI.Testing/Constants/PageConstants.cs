using Milyli.UIAutomation.Relativity.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.PDB.Automated.UI.Testing.Constants
{
	class PageConstants
	{
		//In the future, this is where we should put the object targets for the Charts/Graphs on the individual metric pages.

		//Access PDB Tabs
		public const string PerformanceDashboardTab = "Performance Dashboard";
		public const string QualityOfServiceTab = "Quality of Service";
		public const string RecoverabilityIntegrityTab = "Recoverability/Integrity";
		public const string BackfillConsoleTab = "Backfill Console";

		//Switch To Frames
		public const string InnerFrame = "_externalPage";

		public static void WaitUntil(Func<bool> cm, int seconds)
		{
			new WebDriverWait(Configuration.Driver, TimeSpan.FromSeconds(seconds)).Until(_ => cm());
		}
	}
}
