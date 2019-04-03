using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace kCura.PDD.Web
{
    public partial class ConfirmationPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TitleText.InnerText = Request.QueryString["title"];
            MessageText.InnerText = Request.QueryString["message"];
            WindowId.Value = Request.QueryString["id"];
            WarningMessage.InnerText =  Request.QueryString["warningMessage"];
        }
    }
}