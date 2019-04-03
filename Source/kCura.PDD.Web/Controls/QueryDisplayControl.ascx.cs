using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace kCura.PDD.Web.Controls
{
	public partial class QueryDisplayControl : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Page_PreRender(object sender, EventArgs e)
		{

		}

		protected void QueryDataGridView_RowDataBound(object sender, EventArgs e)
		{

		}

		#region Properties

		public String StartDateViewStateKey = "StartDateQueryDisplayControl";
		public DateTime? _StartDate = null;
		public DateTime StartDate
		{
			get
			{
				if (null == _StartDate)
				{
					if (null != Context.Items[StartDateViewStateKey])
					{
						_StartDate = (DateTime)Context.Items[StartDateViewStateKey];
					}
					else
					{
						_StartDate = DateTime.Now;
					}
				}
				return _StartDate.Value;
			}
			set
			{
				_StartDate = value;
				Context.Items[StartDateViewStateKey] = value;
			}
		}


		public String EndDateViewStateKey = "EndDateQueryDisplayControl";
		public DateTime? _EndDate = null;
		public DateTime EndDate
		{
			get
			{
				if (null == _EndDate)
				{
					if (null != Context.Items[EndDateViewStateKey])
					{
						_EndDate = (DateTime)Context.Items[EndDateViewStateKey];
					}
					else
					{
						_EndDate = DateTime.Now;
					}
				}
				return _EndDate.Value;
			}
			set
			{
				_EndDate = value;
				Context.Items[EndDateViewStateKey] = value;
			}
		}


		public List<long> Workspaces
		{
			get;
			set;
		}

		#endregion


	}
}