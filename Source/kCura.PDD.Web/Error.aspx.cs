using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using kCura.PDD.Web.Enum;

namespace kCura.PDD.Web
{
	public partial class Error : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//Hide everything by default...
			ErrorGeneral.Visible = false;
			ReportDataPending.Visible = false;
			SyncError.Visible = false;
			AccessError.Visible = false;
			Error51773.Visible = false;

			//Parse error type from query string
			var errorDetails = Request.QueryString["mode"];
			var mode = ErrorMode.General;
			System.Enum.TryParse(errorDetails, true, out mode);

			//Show the appropriate HTML
			switch (mode)
			{
				case ErrorMode.ReportDataPending:
					ReportDataPending.Visible = true;
					break;
				case ErrorMode.AssemblySyncError:
					SyncError.Visible = true;
					break;
				case ErrorMode.Disabled:
					Error51773.Visible = true;
					break;
				case ErrorMode.AccessDenied:
					AccessError.Visible = true;
					break;
				default:
					ErrorGeneral.Visible = true;
					break;
			}
		}
	}
}