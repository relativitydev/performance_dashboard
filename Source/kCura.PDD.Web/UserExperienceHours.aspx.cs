using System;
using System.Linq;
using System.Web.Script.Serialization;
using kCura.PDD.Web.Constants;
using kCura.PDD.Web.Models.BISSummary;
using kCura.PDB.Service;
using kCura.PDB.Core.Interfaces.Services;

namespace kCura.PDD.Web
{
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Services;

	public partial class UserExperienceHours : PageBase
	{
		#region Private Members
		private readonly UserExperienceService _userExperience;
		private readonly IQualityIndicatorService _indicatorService;
		#endregion

		public UserExperienceHours()
			: base(true)
		{
			_userExperience = new UserExperienceService(this.SqlRepo);
			_indicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(this.SqlRepo.ConfigurationRepository));
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			var state = Session[DataTableSessionConstants.UserExperienceHoursState] as UserExperienceHoursViewModel;
			var serverParam = Request.Params["Server"] ?? string.Empty;
			var workspaceParam = Request.Params["Workspace"] ?? string.Empty;

			//If any query string parameters differ from previous model state filters, build a new model
			if (state != null && (state.FilterConditions.Server.ToString() != serverParam || state.FilterConditions.Workspace != workspaceParam))
				state = null;
			var model = state ?? new UserExperienceHoursViewModel();

			Initialize(model, serverParam, workspaceParam);
			var server = _userExperience.ListDatabaseServers().FirstOrDefault(x => x.ArtifactId == model.FilterConditions.Server);
			var serverName = server != null
				? server.Name
				: model.FilterConditions.Server.ToString();

			var scores = _userExperience.GetOverallScores();

			QoSNavButton.HRef = QosNavigationUrl;

			QuarterlyScore.Attributes["class"] = _indicatorService.GetCssClassForScore(scores.QuarterlyUserExperienceScore, true);
			QuarterlyScore.InnerText = _indicatorService.GetIndicatorForScore(scores.QuarterlyUserExperienceScore) != PDB.Core.Enumerations.QualityIndicator.None ?
										scores.QuarterlyUserExperienceScore.ToString() : "N/A";

			QuarterlyScore.Attributes["href"] = QosNavigationUrl;

			ReportHeader.InnerHtml = $"Hours View Report<br/>Server: {serverName} ({model.FilterConditions.Server})";

			DateFormatString.Value = DateFormat;
			TimeFormatString.Value = TimeFormat;

			var timezoneOffset = RequestService.GetTimezoneOffset(this.Request);

			startDate.Value = model.GridConditions.StartDate.HasValue
				? model.GridConditions.StartDate.Value.AddMinutes(timezoneOffset).ToShortDateString()
				: GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).AddMinutes(timezoneOffset).ToShortDateString();
			endDate.Value = model.GridConditions.EndDate.HasValue
				? model.GridConditions.EndDate.Value.AddMinutes(timezoneOffset).ToShortDateString()
				: DateTime.UtcNow.AddMinutes(timezoneOffset).ToShortDateString();
		}

		private void Initialize(UserExperienceHoursViewModel model, string serverParam, string workspaceParam)
		{
			int serverId;
			if (int.TryParse(serverParam, out serverId))
				model.FilterConditions.Server = serverId;
			if (!string.IsNullOrEmpty(workspaceParam))
				model.FilterConditions.Workspace = workspaceParam;

			var json = new JavaScriptSerializer().Serialize(model);
			VarscatState.Value = json;

			TimezoneOffset.Value = RequestService.GetTimezoneOffset(this.Request).ToString();
			SampleStart.Value = base.GlassInfo.MinSampleDate.GetValueOrDefault(DateTime.UtcNow).ToString("s");
		}
	}
}