namespace kCura.PDD.Web.Services
{
	using System.Web;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Service.BISSummary;

	public static class UrlHelper
	{
		public static string GetPageUrl(BestInServiceReportingService reportingService, string tabName, string pageName, string additionalParamerters = null)
		{
			return GetPageUrl(reportingService.GetTabArtifactId(tabName), pageName, additionalParamerters);
		}

		public static string GetPageUrl(int tabId, string pageName, string additionalParamerters = null)
		{
			var pageUrl = string.Empty;

			switch (pageName.ToLower())
			{
				// new Vue.js pages
				case "servicequality":
				case "configuration":
				case "backfill":
					pageUrl = HttpUtility.UrlEncode(
						$"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/mvc/{pageName}");
					break;
				default:
					pageUrl = HttpUtility.UrlEncode(
						$"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/{pageName}.aspx?StandardsCompliance=true");
					break;
			}

			pageUrl = additionalParamerters == null ? pageUrl : pageUrl + $"&{additionalParamerters}";
			return $"/Relativity/External.aspx?" +
			       $"AppID=-1" +
			       $"&ArtifactID=-1" +
			       $"&SelectedTab={tabId}" +
			       $"&DirectTo={pageUrl}";
		}
	}
}