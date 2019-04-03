namespace kCura.PDD.Web
{
	using System;
	using System.Linq;
	using System.Web;
	using kCura.PDB.Core.Models.BISSummary.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDD.Web.Factories;
	using kCura.PDD.Web.Services;

	public partial class DbccTarget : PageBase
	{
		#region Protected Members
		protected DbccTargetService _service;
		protected IAuthenticationService _authService;
		#endregion

		public DbccTarget()
		{
			_service = new DbccTargetService(new SqlServerRepository(connectionFactory));
			_authService = new AuthenticationServiceFactory().GetService();
        }

		#region Events

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				RefreshTargetList();
				downloadStandardViewButton.HRef = ResolveUrl(VirtualPathUtility.ToAbsolute("~/QoS_DBCCLog.txt"));
				postCount.Value = "1";
			}
			else
				SaveSettings(sender, e);
		}

		#endregion

		#region View Model Settings

		private void RefreshTargetList()
		{
			_service.RefreshTargets();
			var targets = _service.ListTargets().OrderBy(x => x.Server).ToList();
			servers.DataSource = targets;
			servers.DataBind();
		}

		#endregion

		#region Form Handling
		/// <summary>
		/// Save form data to the configuration table if it's valid
		/// Indicate a response on the client side
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SaveSettings(object sender, EventArgs e)
		{
			postCount.Value = (int.Parse(postCount.Value) + 1).ToString();
			var target = new DbccTargetInfo
			{
				Id = int.Parse(targetId.Value),
				Server = serverName.Value.Trim(),
				Database = databaseName.Value.Trim(),
				IsActive = isActive.Checked
			};

			//Validate settings
			var result = _service.ValidateTarget(target);
			if (!result.Valid)
			{
				validationMessage.InnerText = result.Details;

				//Display warning pane for validation message
				displayWarningMessage.Visible = true;

				if (!ClientScript.IsStartupScriptRegistered("ExecuteOnWarning"))
				{
					Page.ClientScript.RegisterStartupScript(GetType(), "ExecuteOnWarning", "ExecuteOnWarning();", true);
				}
				return;
			}

			//Passed validation - save settings as this user
			var userId = _authService.GetUserId();
			_service.SaveTarget(target, userId);

			//Settings saved - display success pane
			displaySuccessMessage.Visible = true;
			successMessage.InnerText = String.Format("Target details for {0} have been saved.", target.Server);
			if (!ClientScript.IsStartupScriptRegistered("ExecuteOnSuccess"))
			{
				Page.ClientScript.RegisterStartupScript(GetType(), "ExecuteOnSuccess", "ExecuteOnSuccess();", true);
			}
			RefreshTargetList();
		}
		#endregion
	}
}