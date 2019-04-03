namespace kCura.PDD.Web
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Security.Principal;
	using System.Text;
	using global::Relativity.CustomPages;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Deployment;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Installation;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Services;
	using kCura.PDD.Web.Factories;
	using kCura.PDD.Web.Models;
	using kCura.PDD.Web.Services;

	public partial class AdministrationInstall : System.Web.UI.Page
	{
		#region Protected Members
		protected bool reloadNeeded { get; set; }
		protected AdministrationInstallationModel _pageModel { get; set; }
		protected AdministrationInstallationService _service { get; set; }
		protected IAuthenticationService _authService;
		protected IServerRepository _serverRepository { get; set; }
		protected ISqlServerRepository sqlServerRepository { get; set; } // Only used for 'AdminScriptsInstalled'
		#endregion

		public AdministrationInstall()
		{
			//register the prerender event
			this.PreRender += Page_PreRender;
			var logger = new TextLogger();
			_pageModel = new AdministrationInstallationModel();

			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			this.sqlServerRepository = new SqlServerRepository(connectionFactory);
			_serverRepository = new ServerRepository(connectionFactory);
			var databaseDeployer = new DatabaseDeployer(
				new DatabaseMigratorFactory(connectionFactory),
				new ExceptionMigrationResultHandler(logger));
			var administrationInstallationRepository = new AdministrationInstallationRepository(connectionFactory);
			var refreshServerService = new RefreshServerService(logger, new ResourceServerRepository(connectionFactory));
			_service = new AdministrationInstallationService(databaseDeployer, _serverRepository, refreshServerService, administrationInstallationRepository);
			_authService = new AuthenticationServiceFactory().GetService();
		}

		#region Events

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{ //initialize page
				InitializePage();
			}
			else
			{//get page state
				GetCurrentPage();
			}
		}

		/// <summary>
		/// Page PreRender bind information 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindPageModelToUI();
		}

		#endregion

		#region User Interface
		/// <summary>
		/// Binds View Model to UI
		/// </summary>
		private void BindPageModelToUI()
		{
			//turn all ui elements off
			displayErrorMessage.Visible = _pageModel.DisplayErrorMessage;
			displayWarningMessage.Visible = !string.IsNullOrEmpty(_pageModel.ExceptionMessage);
            fullExceptionMessage.InnerText = _pageModel.ExceptionMessage;
			InstallationProgressPane.Visible = _pageModel.DisplayInstallationProgressPane;
			displaySuccessMessage.Visible = _pageModel.DisplaySuccessMessage;
			scriptInstallationSubmitButton.Visible = _pageModel.DisplayScriptInstallationSubmitButton;
			scriptInstallationSubmitMockButton.Visible = _pageModel.DisplayScriptInstallationSubmitMockButton;

			var scriptsInstalled = this.sqlServerRepository.AdminScriptsInstalled();
            scriptsInstallationBackButton.Visible = scriptsInstalled;

			//set pagemodel username and password to empty
			_pageModel.DatabaseUsername = string.Empty;
			_pageModel.DatabasePassword = string.Empty;

			//set UI username and password to empty
			databaseUsername.Text = string.Empty;
			databasePassword.Text = string.Empty;

			this.scriptInstallationSteps.DataSource = _pageModel.InstallStepList;
			this.scriptInstallationSteps.DataBind();
		}
		#endregion

		#region View Model Settings

		private void InitializePage()
		{
			//turn all ui elements off
			_pageModel.DisplayErrorMessage = false;
			_pageModel.DisplayInstallationProgressPane = false;
			_pageModel.DisplaySuccessMessage = false;
            _pageModel.ExceptionMessage = string.Empty;

			//set username and password to empty
			_pageModel.DatabaseUsername = string.Empty;
			_pageModel.DatabasePassword = string.Empty;

			_pageModel.DisplayScriptInstallationSubmitButton = true;
			_pageModel.DisplayScriptInstallationSubmitMockButton = false;
			//initialize repeater
			_pageModel.InstallStepList = new List<InstallationStep>();

			if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated &&
				System.Web.HttpContext.Current.User.Identity is WindowsIdentity)
			{
				useWinAuth.Checked = true;
				databaseWinAuthName.Text = System.Web.HttpContext.Current.User.Identity.Name;
			}
			else
			{
				//We don't have the user's Windows identity, but we can still try WinAuth with the user running the AppPool
				databaseWinAuthName.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			}
		}

		/// <summary>
		/// Gets the current page on postback
		/// </summary>
		private void GetCurrentPage()
		{
			//turn all ui elements off
			_pageModel.DisplayErrorMessage = displayErrorMessage.Visible;
			_pageModel.DisplayInstallationProgressPane = InstallationProgressPane.Visible;
			_pageModel.DisplaySuccessMessage = displaySuccessMessage.Visible;

			//set username and password to empty
			_pageModel.DatabaseUsername = string.Empty;
			_pageModel.DatabasePassword = string.Empty;

			_pageModel.DisplayScriptInstallationSubmitButton = scriptInstallationSubmitButton.Visible;
			_pageModel.DisplayScriptInstallationSubmitMockButton = scriptInstallationSubmitMockButton.Visible;

			//initialize repeater
			_pageModel.InstallStepList = new List<InstallationStep>();
		}

		#endregion

		#region Form Handling
		
		/// <summary>
		/// When the Submit Installation Button Clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SubmitInstallationForm(object sender, EventArgs e)
		{
			WindowsImpersonationContext context = null;

			try
			{
				if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated &&
					System.Web.HttpContext.Current.User.Identity is WindowsIdentity &&
					useWinAuth.Checked)
				{
					//When using WinAuth and the user's Windows identity is available, try to impersonate the user
					try
					{
						context = ((WindowsIdentity)System.Web.HttpContext.Current.User.Identity).Impersonate();
					}
					catch(Exception ex)
					{
						//Impersonation failed, continue as the AppPool identity and note this in the detailed logs
						_pageModel.InstallStepList.Add(new InstallationStep
						{
							InstallationStepValue = String.Format("Using {0} for deployment because the user identity could not be impersonated: {1}",
								System.Security.Principal.WindowsIdentity.GetCurrent().Name,
								ex.Message)
						});
					}
				}

                //Validate connectivity and DBCC permissions
				string exceptionMessage = string.Empty;
				var credentials = new GenericCredentialInfo
				{
					UserName = databaseUsername.Text,
					Password = databasePassword.Text,
					UseWindowsAuthentication = useWinAuth.Checked
				};

				try
                {
                    GetFormMasterDBCredentialsAndValidate(credentials);
                }
                catch (Exception exception)
                {
                    exceptionMessage = exception.ToString();
                }

                if (!string.IsNullOrEmpty(exceptionMessage))
                {
                    _pageModel.ExceptionMessage = exceptionMessage;
                    // bad username or password
                    if (!ClientScript.IsStartupScriptRegistered("ExecuteOnWarning"))
                    {
                        Page.ClientScript.RegisterStartupScript(GetType(), "ExecuteOnWarning", "ExecuteOnWarning();", true);
                    }
                }
                else
                {
					//disable submit button
					_pageModel.DisplayScriptInstallationSubmitButton = false;
					_pageModel.DisplayScriptInstallationSubmitMockButton = true;
					_pageModel.DisplayInstallationProgressPane = true;

	                
					var results = _service.InstallScripts(credentials);

					var sb = new StringBuilder();
					foreach (var message in results.Messages)
					{
						//Add each message to the export log, and add messages that aren't verbose to the install steps
						sb.AppendLine(message.Text);
						if (!message.Verbose)
							_pageModel.InstallStepList.Add(new InstallationStep { InstallationStepValue = message.Text });
					}

					Session["Export"] = sb.ToString();
					if (results.Success)
					{
						//scripts have been installed
						_pageModel.DisplaySuccessMessage = true;
						if (!ClientScript.IsStartupScriptRegistered("ExecuteOnSuccess"))
						{
							Page.ClientScript.RegisterStartupScript(GetType(), "ExecuteOnSuccess", "ExecuteOnSuccess();", true);
						}
					}
					else
					{
						//scripts have not been installed
						_pageModel.DisplayErrorMessage = true;
						if (!ClientScript.IsStartupScriptRegistered("ExecuteOnError"))
						{
							Page.ClientScript.RegisterStartupScript(GetType(), "ExecuteOnError", "ExecuteOnError();", true);
						}
					}
				}
			}
			catch
			{
				//Preventing exception filters from catching the exception instead of us and continuing to run under impersonation...
				if (context != null)
					context.Undo();
				throw;
			}
			finally
			{
				//Always remember to reverse impersonation!
				if (context != null)
					context.Undo();
			}
		}

		/// <summary>
		/// Installation Export
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void RequestExportOfInstallationStep(object sender, EventArgs e)
		{
			//requestExportOfInstallationButton
			var executionExport = (string)(Session["Export"]);
			var fileName = string.Format("pdb_admin_install_scripts_{0}", DateTime.Now.ToString());
			Response.Clear();
			Response.Charset = "";
			Response.ContentType = "text/plain";
			Response.AppendHeader("content-disposition", "attachment; filename=" + fileName + ".dat");
			Response.Write(executionExport);
			Response.End();
		}

		/// <summary>
		/// Validates The DB Credentials The User Passes In
		/// </summary>
		/// <returns></returns>
		private bool GetFormMasterDBCredentialsAndValidate(GenericCredentialInfo sqlCredentials)
		{
			_pageModel.DatabaseUsername = databaseUsername.Text;
			_pageModel.DatabasePassword = databasePassword.Text;
            return _service.CredentialsAreValid(sqlCredentials);
		}
		#endregion
	}
}