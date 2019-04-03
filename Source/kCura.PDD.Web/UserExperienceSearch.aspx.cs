using System;
using System.Web.Script.Serialization;
using kCura.PDD.Web.Constants;
using kCura.PDD.Web.Models.BISSummary;
using kCura.PDB.Core.Interfaces.Services;
using kCura.PDB.Service;

namespace kCura.PDD.Web
{
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Services;

	public partial class UserExperienceSearch : PageBase
	{
		#region Private Members
		private readonly UserExperienceService _userExperience;
		private readonly IQualityIndicatorService _indicatorService;
		#endregion

		public UserExperienceSearch()
			: base(true)
		{
			_userExperience = new UserExperienceService(this.SqlRepo);
			_indicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(this.SqlRepo.ConfigurationRepository));
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.UserExperienceSearchState] as UserExperienceSearchViewModel;
			var workspaceParam = Request.Params["CaseArtifactID"] ?? string.Empty;
			var searchParam = Request.Params["Search"] ?? string.Empty;

			//If any query string parameters differ from previous model state filters, build a new model
			if (state != null && (state.FilterConditions.CaseArtifactId.ToString() != workspaceParam || state.FilterConditions.Search != searchParam))
				state = null;
			var model = state ?? new UserExperienceSearchViewModel();

			Initialize(model, workspaceParam, searchParam);

			var scores = _userExperience.GetOverallScores();

			QoSNavButton.HRef = QosNavigationUrl;

			QuarterlyScore.Attributes["class"] = _indicatorService.GetCssClassForScore(scores.QuarterlyUserExperienceScore, true);
			QuarterlyScore.InnerText = _indicatorService.GetIndicatorForScore(scores.QuarterlyUserExperienceScore) != PDB.Core.Enumerations.QualityIndicator.None ?
										scores.QuarterlyUserExperienceScore.ToString() : "N/A";
			QuarterlyScore.Attributes["href"] = QosNavigationUrl;

			var workspaceName = _userExperience.ReadWorkspaceFriendlyName(model.FilterConditions.CaseArtifactId);
			ReportHeader.InnerHtml = $"Search Details Report<br/>Workspace: {workspaceName} ({model.FilterConditions.CaseArtifactId})";

			DateFormatString.Value = DateFormat;
			TimeFormatString.Value = TimeFormat;

			serverSelection.Value = _userExperience.GetWorkspaceServerId(model.FilterConditions.CaseArtifactId).ToString();
			var timezoneOffset = RequestService.GetTimezoneOffset(this.Request);
			startDate.Value = model.GridConditions.StartDate.HasValue
				? model.GridConditions.StartDate.Value.AddMinutes(timezoneOffset).ToShortDateString()
				: GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).AddMinutes(timezoneOffset).ToShortDateString();
			endDate.Value = model.GridConditions.EndDate.HasValue
				? model.GridConditions.EndDate.Value.AddMinutes(timezoneOffset).ToShortDateString()
				: DateTime.UtcNow.AddMinutes(timezoneOffset).ToShortDateString();
		}

		private void Initialize(UserExperienceSearchViewModel model, string workspaceParam, string searchParam)
		{
			int workspaceId;
			if (int.TryParse(workspaceParam, out workspaceId))
				model.FilterConditions.CaseArtifactId = workspaceId;
			if (!string.IsNullOrEmpty(searchParam))
				model.FilterConditions.Search = searchParam;

			var json = new JavaScriptSerializer().Serialize(model);
			VarscatState.Value = json;

			TimezoneOffset.Value = RequestService.GetTimezoneOffset(this.Request).ToString();
			SampleStart.Value = base.GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).ToString("s");
		}
	}
}