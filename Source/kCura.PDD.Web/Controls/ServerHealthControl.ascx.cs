namespace kCura.PDD.Web.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using DevExpress.Web.ASPxEditors;
	using DevExpress.XtraCharts;
	using System.Globalization;
	using DevExpress.Web.ASPxSplitter;
	using DevExpress.Web.ASPxGridView;
	using System.Web.UI.WebControls;
	using kCura.PDD.Web.Enum;
	using DevExpress.XtraPrinting;
	using System.Web;
	using System.IO;
	using global::Relativity.CustomPages;
	using kCura.PDD.Web.Models.BISSummary;
	using kCura.PDD.Web.Services;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.HealthChecks;
	using kCura.PDB.Core.Models.HealthChecks;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.BISSummary;
	using kCura.PDB.Service.HealthChecks;

	public partial class ServerHealthControl : System.Web.UI.UserControl, IViewHealth
	{
		private IQualityIndicatorService _indicatorService;
		private UserExperienceService _service;

		SqlHealthPresenter presenter;
		protected ImageButton ShowAllButton;
		public ASPxGridView DynamicHealthGridView = new ASPxGridView();
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			presenter = new SqlHealthPresenter(this);
			DevExpress.Web.ASPxClasses.ASPxWebControl.SetIECompatibilityMode(9);

			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var sqlRepo = new SqlServerRepository(connectionFactory);
			_service = new UserExperienceService(sqlRepo);
			_indicatorService = new QualityIndicatorService(new QualityIndicatorConfigurationService(new ConfigurationRepository(connectionFactory)));
		}

		public string SessionStartDate
		{
			get
			{
				if (HealthControl == HealthControlType.ApplicationHealth)
					return "AppPerformance_StartDate";
				else
					return "ServerHealth_StartDate";
			}
		}
		public string SessionEndDate
		{
			get
			{
				if (HealthControl == HealthControlType.ApplicationHealth)
					return "AppPerformance_EndDate";
				else
					return "ServerHealth_EndDate";
			}
		}
		public const string RequestStartDate = "startDate";
		public const string RequestEndDate = "endDate";
		public const string RequestMeasureType = "measureType";

		public HealthControlType HealthControl { get; set; }
		public string GetExportFileName
		{
			get
			{
				if (HealthControl == HealthControlType.ApplicationHealth)
				{
					return "ApplicationPerformance";
				}
				else
				{
					return String.Format(_measureType.ToString() + "{0}", "Health");
				}
			}
		}

		/// <summary>
		/// Get data from task using presenter and bind all controls
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			DateRangeIsEuroFormat();
			HideAllGrids();
			SetMeasureType();

			//hide varscat for now
			this.QueryPerformanceImageButton.Visible = false;

			if (!IsPostBack)
			{
				if (Page.Request.Cookies["TimeZoneOffset"] == null)
				{
					var cookie = new HttpCookie("TimeZoneOffset", $"{TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes}")
					{
						Expires = DateTime.Now.AddDays(1)
					};
					Page.Request.Cookies.Add(cookie);
				}
				var ckTimeZoneOffset = Request.Cookies["TimeZoneOffset"];
				hdnTimeZoneMinutes.Value = ckTimeZoneOffset.Value;
				this.TimeZoneOffset = GetTimeZoneOffsets();

				if (_measureType == MeasureTypes.AppHealth)
				{
					ltrTitle.Text = "Application Performance";
					ServerAreaComboBoxPlaceHolder.Visible = false;
				}
				else
				{
					MeasureTypes measureType;
					if (Request.Params[RequestMeasureType] != null && System.Enum.TryParse(Request.Params[RequestMeasureType], out measureType))
					{
						if (measureType != MeasureTypes.AppHealth && measureType != MeasureTypes.BIS)
						{
							ServerAreaComboBox.SelectedIndex = ServerAreaComboBox.Items.IndexOfValue(Request.Params[RequestMeasureType]);
							_measureType = measureType;
						}
					}
					ServerAreaComboBoxPlaceHolder.Visible = true;
					ltrTitle.Text = "Server Health";
				}

				//Capture start and end dates from the query string parameters or the session (in priority order)...
				//Query string dates currently won't persist in the session (until "Go" is clicked), but we can overwrite the session data if we want to.
				DateTime start, end;
				if (Request.Params[RequestStartDate] != null && DateTime.TryParse(Request.Params[RequestStartDate], out start))
				{
					this.StartDate = start;
					StartDateEdit.Value = this.StartDate;
				}
				else if (Session[SessionStartDate] != null && !string.IsNullOrWhiteSpace(Session[SessionStartDate].ToString()))
				{
					this.StartDate = DateTime.Parse(Session[SessionStartDate].ToString());
					StartDateEdit.Value = this.StartDate;
				}

				if (Request.Params[RequestEndDate] != null && DateTime.TryParse(Request.Params[RequestEndDate], out end))
				{
					this.EndDate = end;
					EndDateEdit.Value = this.EndDate;
				}
				else if (Session[SessionEndDate] != null && !string.IsNullOrWhiteSpace(Session[SessionEndDate].ToString()))
				{
					this.EndDate = DateTime.Parse(Session[SessionEndDate].ToString());
					EndDateEdit.Value = this.EndDate;
				}

				//Wire up navigation
				var qosUrl = UrlHelper.GetPageUrl(_service, Names.Tab.QualityOfService, "ServiceQuality");

				var overall = new ServiceCategory();
				var weekly = new ServiceCategory();
				if (_service.LookingGlassHasRun())
				{
					var scores = _service.GetOverallScores();
					overall.Score = scores.QuarterlyScore;
					weekly.Score = scores.WeeklyScore;
				}
				else
				{
					overall.Score = -1;
					weekly.Score = -1;
				}

				QoSNavButton.HRef = qosUrl;

				BestInServiceScore.Attributes["class"] = _indicatorService.GetCssClassForScore(overall.Score, true);
				BestInServiceScore.InnerText = _indicatorService.GetIndicatorForScore(overall.Score) != PDB.Core.Enumerations.QualityIndicator.None ?
										overall.Score.ToString() : "N/A";
				BestInServiceScore.Attributes["href"] = qosUrl;

				WeeklyScore.Attributes["class"] = _indicatorService.GetCssClassForScore(weekly.Score, true);
				WeeklyScore.InnerText = _indicatorService.GetIndicatorForScore(weekly.Score) != PDB.Core.Enumerations.QualityIndicator.None ? 
										weekly.Score.ToString() : "N/A";
				WeeklyScore.Attributes["href"] = qosUrl;

				var rootPane = HealthSplitter.Panes[0];
				ChartImageButton.ImageUrl = "~/Images/Chart_on.png";
				rootPane.Panes[1].Collapsed = false;

				Session[ColumnsListKey] = null;
				Session[HealthDataKey] = null;
				ShowHideFiltersLinkSetting(true);
				PageDropDownList.Value = "25";
				BindCombo();
				BindGrid();

				foreach (var item in ColumnsList)
				{

					if (HealthControl == HealthControlType.ApplicationHealth)
					{
						DynamicHealthGridView.Columns[(item as KeyValueDesc).Value].Caption = (item as KeyValueDesc).Description;
					}
				}
				SetDisplayLabelDateTime();
				Page.ClientScript.RegisterStartupScript(this.GetType(), "_SetTimeZoneDate", "<script type='text/javascript'>SetTimeZoneDate();</script>");
			}
			else
			{
				if (Session[SessionStartDate] != null && Session[SessionEndDate] != null)
				{
					StartDate = (DateTime?)Session[SessionStartDate];
					EndDate = (DateTime?)Session[SessionEndDate];
				}

				this.TimeZoneOffset = GetTimeZoneOffsets();
				SetDisplayLabelDateTime();

				string eventTarget = Request.Form["__EVENTTARGET"];
				string eventArgument = Request.Form["__EVENTARGUMENT"];

				if (eventTarget.Equals("ShowFiltersLink", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(eventArgument))
				{
					BindGrid();
					ShowAdvanceFilterInGrid(bool.Parse(eventArgument.ToString()));
				}
				if (eventTarget.Equals("ClearAllLink", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(eventArgument))
				{
					BindGrid();
					ClearFilterGrid(bool.Parse(eventArgument.ToString()));
				}
				if (eventTarget.Equals("ApplyFilterLink", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(eventArgument))
				{
					BindGrid();
					ApplyFilterInGrid(bool.Parse(eventArgument.ToString()));
				}
				if (Request.Form["__CALLBACKPARAM"] != null)
				{
					BindGrid();
				}
			}
		}


		#region Events

		/// <summary>
		/// Raise when btnXlsExport is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		protected void btnXlsExport_Click(object sender, EventArgs e)
		{
			BindGrid();
			using (MemoryStream stream = new MemoryStream())
			{
				ExportGrid.WriteXls(stream);
				WriteToResponse(this.GetExportFileName, true, FileFormat.xls, stream);
			}
		}

		/// <summary>
		/// Raise when btnXlsxExport is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnXlsxExport_Click(object sender, EventArgs e)
		{
			BindGrid();
			using (MemoryStream stream = new MemoryStream())
			{
				ExportGrid.WriteXlsx(stream);
				WriteToResponse(this.GetExportFileName, true, FileFormat.xlsx, stream);
			}
		}

		/// <summary>
		/// Raise when btnCsvExport is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCsvExport_Click(object sender, EventArgs e)
		{
			BindGrid();
			using (MemoryStream stream = new MemoryStream())
			{
				ExportGrid.WriteCsv(stream);
				WriteToResponse(this.GetExportFileName, true, FileFormat.csv, stream);
			}
		}

		/// <summary>
		/// Raise when server area combo box selected index changed executed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ServerAreaComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SplitterPane rootPane = HealthSplitter.Panes[0];

			Session[ColumnsListKey] = null;
			Session[HealthDataKey] = null;
			BindCombo();
			HideAllGrids();
			ShowHideFiltersLinkSetting(true);
			PageDropDownList.Value = "25";
			ChartTypeComboBox.Value = "1";
			BindGrid();
			ClearFilterGrid(true);
			//rootPane.Panes[0].Collapsed = true;
			//ShowHideGrid();
			//rootPane.Panes[1].Collapsed = false;
			//ShowHideChart(sender);
			DynamicHealthGridView.Selection.UnselectAll();
			SetDisplayLabelDateTime();
		}

		/// <summary>
		/// Raise when chart combo box selected index changed executed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void HealthChartControl_CustomCallback(object sender, DevExpress.XtraCharts.Web.CustomCallbackEventArgs e)
		{
			FilterGraph();
		}

		/// <summary>
		/// used to refresh graph based on passed parameter value changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ShowDataButton_Click(object sender, EventArgs e)
		{
			Session[HealthDataKey] = null;
			this.TimeZoneOffset = GetTimeZoneOffsets();

			if (StartDateEdit.Value != null && EndDateEdit.Value != null)
			{
				this.StartDate = Convert.ToDateTime(StartDateEdit.Value);
				this.EndDate = Convert.ToDateTime(EndDateEdit.Value);
			}
			else
			{
				this.StartDate = null;
				this.EndDate = null;
			}

			Session[SessionStartDate] = this.StartDate;
			Session[SessionEndDate] = this.EndDate;
			BindGrid();
			SplitterPane rootPane = HealthSplitter.Panes[0];
			if (rootPane.Panes[1].Collapsed == false)
			{
				FilterGraph();
			}
			SetDisplayLabelDateTime();
			DateRangesComboBox.SelectedIndex = 0;
		}

		/// <summary>
		/// used to clear start date & end date
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ClearDataButton_Click(object sender, EventArgs e)
		{
			StartDateEdit.Value = null;
			EndDateEdit.Value = null;

			Session[SessionStartDate] = null;
			Session[SessionEndDate] = null;

			ShowDataButton_Click(this, null);
		}

		/// <summary>
		/// Used to toggle chart panel on/off
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ChartImageButton_Click(object sender, EventArgs e)
		{
			BindGrid();
			ShowHideChart(sender);
		}

		/// <summary>
		/// Used to toggle grid panel on/off
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void GridImageButton_Click(object sender, EventArgs e)
		{
			BindGrid();
			ShowHideGrid();
		}

		/// <summary>
		/// Make graph to fit with its container.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void FitToScreenImageButton_Click(object sender, EventArgs e)
		{
			BindGrid();
			FilterGraph();
			SetChartFitToContainer();
		}

		protected void QueryPerformanceImageButton_Click(object sender, EventArgs e)
		{
			BindGrid();
			FilterGraph();

			if (ChartPanel.Visible)
			{
				//QueryPerformanceImageButton.AlternateText = "Back to Chart";
				QueryPerformanceImageButton.ToolTip = "Back to Chart";
				QueryPerformanceImageButton.ImageUrl = ResolveUrl("~/Images/back_arrow.png");
				ShowAllButton.Visible = true;
			}
			else
			{
				//QueryPerformanceImageButton.AlternateText = "Query Performance";
				QueryPerformanceImageButton.ToolTip = "Query Performance";
				QueryPerformanceImageButton.ImageUrl = ResolveUrl("~/Images/gear.png");
				ShowAllButton.Visible = false;
			}

			ChartPanel.Visible = !ChartPanel.Visible;
			QueryPerformancePanel.Visible = !QueryPerformancePanel.Visible;

			Context.Items[QueryPerformanceData.StartDateViewStateKey] = this.StartDate;
			Context.Items[QueryPerformanceData.EndDateViewStateKey] = this.EndDate;
			this.QueryPerformanceData.Workspaces = this.GridSelectedRows;
		}

		/// <summary>
		/// Raise when grid page size is changed executed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void PageDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
			FilterGraph();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Set date range display label
		/// </summary>

		private void SetDisplayLabelDateTime()
		{
			DateTime dtStart = DateTime.UtcNow;
			DateTime dtEnd = DateTime.UtcNow;
			if (this.StartDate != null && this.EndDate != null)
			{
				if (this.StartDate != this.EndDate)
				{
					dtStart = this.StartDate.Value;
					dtEnd = this.EndDate.Value.AddDays(1).AddSeconds(-1);
				}
				else
				{
					dtStart = this.StartDate.Value;
					dtEnd = this.EndDate.Value.AddDays(1).AddSeconds(-1);
				}
			}
			else
			{
				dtStart = Convert.ToDateTime(hdnTimeZoneMinutes.Attributes["UTCDate"]).AddMinutes(this.TimeZoneOffset).AddDays(-1);
				dtEnd = Convert.ToDateTime(hdnTimeZoneMinutes.Attributes["UTCDate"]).AddMinutes(this.TimeZoneOffset).AddSeconds(-1);
			}

			lblStartDate.Text = String.Format("{0:d} {0:HH:mm:ss}", dtStart);
			lblEndDate.Text = String.Format("{0:d} {0:HH:mm:ss}", dtEnd);
		}

		/// <summary>
		/// Export grid data
		/// </summary>
		protected void WriteToResponse(string fileName, bool saveAsFile, FileFormat fileFormat, MemoryStream stream)
		{
			if (Page == null || Page.Response == null) return;
			string disposition = saveAsFile ? "attachment" : "inline";
			Page.Response.Clear();
			Page.Response.Buffer = false;
			Page.Response.AppendHeader("Content-Type", string.Format("application/{0}", fileFormat.ToString()));
			Page.Response.AppendHeader("Content-Transfer-Encoding", "binary");
			Page.Response.AppendHeader("Content-Disposition", string.Format("{0}; filename={1}.{2}", disposition,
			HttpUtility.UrlEncode(fileName).Replace("+", "%20"), fileFormat));
			if (stream.Length > 0)
				Page.Response.BinaryWrite(stream.ToArray());
			Page.Response.End();
		}

		/// <summary>
		/// Set Measure Type
		/// </summary>
		private void SetMeasureType()
		{
			if (HealthControl == HealthControlType.ApplicationHealth)
			{
				_measureType = MeasureTypes.AppHealth;
			}
			else
			{
				_measureType = (MeasureTypes)Convert.ToInt16(ServerAreaComboBox.SelectedItem.Value);
			}
		}

		/// <summary>
		/// Set chart to fit to container
		/// </summary>
		void SetChartFitToContainer()
		{
			string[] WidthHeight = paneWidthHeight.Value.Split(',');
			Unit Width = new Unit(WidthHeight[0]);
			Unit Height = new Unit(WidthHeight[1]);
			HealthChartControl.Width = Width;
			HealthChartControl.Height = Height;
		}

		/// <summary>
		/// Get UTC Minutes diference for local machine
		/// </summary>
		private int GetTimeZoneOffsets()
		{
			return hdnTimeZoneMinutes.Value != string.Empty ? Convert.ToInt32(hdnTimeZoneMinutes.Value) : 0;
		}

		/// <summary>
		/// Get local machine date time
		/// </summary>
		private DateTime GetLocalMachineDate()
		{
			DateTime dtLocal = DateTime.UtcNow.AddMinutes(this.TimeZoneOffset);
			return dtLocal;
		}
		/// <summary>
		/// Hide all grids in page
		/// </summary>
		void HideAllGrids()
		{
			ApplicationHealthGridView.Visible = false;
			RamHealthGridView.Visible = false;
			ProcessorHealthGridView.Visible = false;
			HardDiskHealthGridView.Visible = false;
			SqlServerHealthGridView.Visible = false;
		}

		/// <summary>
		/// Used to toggle grid panel on/off
		/// </summary>        
		private void ShowHideGrid()
		{
			SplitterPane rootPane = HealthSplitter.Panes[0];
			FilterGraph();
			if (rootPane.Panes[0].Collapsed)
			{
				GridImageButton.ImageUrl = "~/Images/Grid_on.png";
				rootPane.Panes[0].Collapsed = false;
			}
			else
			{
				GridImageButton.ImageUrl = "~/Images/Grid_off.png";
				rootPane.Panes[0].Collapsed = true;
			}
		}

		/// <summary>
		/// Bind chart type combo box and display columns combo box
		/// </summary>
		private void BindCombo()
		{
			if (ColumnsList != null)
			{
				ShowColumnsComboBox.DataSource = ColumnsList.OfType<KeyValueDesc>().ToList();
				ShowColumnsComboBox.TextField = "Description";
				ShowColumnsComboBox.ValueField = "Key";
				ShowColumnsComboBox.DataBind();
				ShowColumnsComboBox.SelectedIndex = 0;
			}
		}
		/// <summary>
		/// Bind health grid
		/// </summary>
		private void BindGrid()
		{
			int rowCount = 0;
			var data = HealthData ?? new List<HealthBase>();
			switch (_measureType)
			{
				case MeasureTypes.AppHealth:
					//ApplicationHealth
					//2013-09-23 : ron@milyli : hack put in my whom?
					//HACK : This is the only way I could populate the FailCount property if the two value are the same w/o
					// rewriting the a third of the application

					var applicationHealthSource = kCura.PDD.Web.Mapper.ApplicationHealthMapper.ToApplicationHealth(data);

					rowCount = applicationHealthSource.Count;
					DynamicHealthGridView = ApplicationHealthGridView;
					DynamicHealthGridView.DataSource = applicationHealthSource;
					//AppHealthGridViewValidation.Visible = true;
					//QueryPerformanceImageButton.Visible = true;
					break;
				case MeasureTypes.Ram:
					//RamHealth
					var ramHealthSource = (from item1 in data
																 let ramHealth = item1 as RamHealth
																 group ramHealth by new { ramHealth.Server, ramHealth.ServerType } into g
																 select new
																 {
																	 Id = g.Select(a => a.Id).FirstOrDefault(),
																	 Type = g.Key.ServerType,
																	 Name = g.Key.Server,
																	 PagesPerSec = Math.Round(g.Average(a => a.PagesPerSec < 0 ? 0 : a.PagesPerSec)),
																	 PageFaultsPerSec = Math.Round(g.Average(a => a.PageFaultsPerSec < 0 ? 0 : a.PageFaultsPerSec)),
																	 Percentage = Math.Round(g.Average(a => a.Percentage < 0 ? 0 : a.Percentage))
																 }).ToList();

					rowCount = ramHealthSource.Count;
					DynamicHealthGridView = RamHealthGridView;
					DynamicHealthGridView.DataSource = ramHealthSource;
					break;
				case MeasureTypes.Processor:
					//ProcesserHealth
					var processerHealthSource = (from item in data
																			 let processerHealth = item as ProcesserHealth
																			 group processerHealth by new { processerHealth.Server, processerHealth.ServerType } into g
																			 select new
																			 {
																				 Id = g.Select(a => a.Id).FirstOrDefault(),
																				 Type = g.Key.ServerType,
																				 Name = g.Key.Server,
																				 ServerCore = g.Select(a => a.ServerCore).FirstOrDefault(),
																				 ProcessorTime = Math.Round(g.Average(a => a.ProcesserTime < 0 ? 0 : a.ProcesserTime))
																			 }).OrderBy(a => a.Type).ThenBy(a => a.Name).ToList();

					rowCount = processerHealthSource.Count;
					DynamicHealthGridView = ProcessorHealthGridView;
					DynamicHealthGridView.DataSource = processerHealthSource;
					break;
				case MeasureTypes.HardDisk:
					//HardDiskHealth
					var hardDiskHealthSource = (from item in data
																			let hardDiskHealth = item as HardDiskHealth
																			group hardDiskHealth by new { hardDiskHealth.Server, hardDiskHealth.ServerType, hardDiskHealth.DiskNumber, hardDiskHealth.DriveLetter } into g
																			select new
																			{
																				Id = g.Select(a => a.Id).FirstOrDefault(),
																				Type = g.Key.ServerType,
																				Name = g.Key.Server,
																				ServerDisk = g.Select(a => a.ServerDisk).FirstOrDefault(),
																				DiskNumber = g.Key.DiskNumber,
																				DriveLetter = g.Key.DriveLetter,
																				DiskRead = g.Average(a => a.DiskRead < 0 ? 0 : a.DiskRead).ToString("N3"),
																				DiskWrite = g.Average(a => a.DiskWrite < 0 ? 0 : a.DiskWrite).ToString("N3")
																			}).OrderBy(a => a.Type).ThenBy(a => a.Name).ThenBy(a => a.DiskNumber).ToList();

					rowCount = hardDiskHealthSource.Count;
					DynamicHealthGridView = HardDiskHealthGridView;
					DynamicHealthGridView.DataSource = hardDiskHealthSource;
					break;
				case MeasureTypes.SQLServer:
					//SqlServerHealth
					var sqlServerHealthSource = (from item in data
																			 let sqlServerHealth = item as SqlServerHealth
																			 group sqlServerHealth by new { sqlServerHealth.Server, sqlServerHealth.ServerType } into g
																			 select new
																			 {
																				 Id = g.Select(a => a.Id).FirstOrDefault(),
																				 Type = g.Key.ServerType,
																				 Name = g.Key.Server,
																				 PageLifeExpectancy = Math.Round(g.Average(a => a.PageLifeExpectancy < 0 ? 0 : a.PageLifeExpectancy))
																			 }).ToList();

					rowCount = sqlServerHealthSource.Count;
					DynamicHealthGridView = SqlServerHealthGridView;
					DynamicHealthGridView.DataSource = sqlServerHealthSource;
					break;
				default:
					break;
			}

			DynamicHealthGridView.Visible = true;
			DynamicHealthGridView.DataBind();
			ExportGrid.GridViewID = DynamicHealthGridView.ID;

			#region Grid Paging Setting
			DynamicHealthGridView.SettingsPager.PageSize = Convert.ToInt32(PageDropDownList.Value);
			if (DynamicHealthGridView.SettingsPager.PageSize > 0)
			{
				int _currentPage = (DynamicHealthGridView.PageIndex + 1);
				int _totalPage = (rowCount / DynamicHealthGridView.SettingsPager.PageSize);

				if ((rowCount % DynamicHealthGridView.SettingsPager.PageSize) > 0)
				{
					_totalPage++;
				}

				DynamicHealthGridView.ClientSideEvents.EndCallback = string.Join("", "function(sender, event) {", string.Format("GridCallBackSetting({0});", _totalPage), "}");
				TotalPageLabel.Text = _totalPage.ToString();
				CurrentPageLabel.Text = _currentPage.ToString();
				TotalRecordsLabel.Text = rowCount.ToString();
				GridPagingLinkButtonSetting(FirstDataLink);
				GridPagingLinkButtonSetting(PreviousDataLink);
				GridPagingLinkButtonSetting(NextDataLink);
				GridPagingLinkButtonSetting(LastDataLink);
				if (!IsPostBack)
				{
					this.Page.ClientScript.RegisterStartupScript(GetType(), "GridLoadSetting", string.Format("GridLoadSetting({0})", _totalPage), true);
				}
			}
			#endregion
		}

		/// <summary>
		/// This method is used to get selected workspace from grid and filter chart based on selected workspace
		/// </summary>
		private void FilterGraph()
		{
			List<object> selectedValues;
			object[] rows = new object[DynamicHealthGridView.VisibleRowCount];
			for (int i = 0; i < DynamicHealthGridView.VisibleRowCount; i++)
			{
				rows[i] = DynamicHealthGridView.GetRowValues(i, DynamicHealthGridView.KeyFieldName);
			}
			selectedValues = DynamicHealthGridView.GetSelectedFieldValues(new string[] { "Id" });
			var v = selectedValues.Intersect(rows);
			GridSelectedRows = new List<Int64>(v.Select(p => Convert.ToInt64(p.ToString())));
			PerformActions();
		}

		/// <summary>
		/// This method is used to show hide chart
		/// </summary>
		private void ShowHideChart(object sender)
		{
			SplitterPane rootPane = HealthSplitter.Panes[0];
			if (rootPane.Panes[1].Collapsed)
			{
				FilterGraph();
				ChartImageButton.ImageUrl = "~/Images/Chart_on.png";
				rootPane.Panes[1].Collapsed = false;
				SetChartFitToContainer();
			}
			else
			{
				ChartImageButton.ImageUrl = "~/Images/Chart_off.png";
				rootPane.Panes[1].Collapsed = true;
			}
		}

		/// <summary>
		/// Check for if any item is selected on grid
		/// </summary>
		/// <returns>Return true if any item is selected in the grid</returns>
		private bool IsAnyGridItemSelected()
		{
			List<object> selectedValues;

			selectedValues = DynamicHealthGridView.GetSelectedFieldValues(new string[] { "Id" });

			return (selectedValues.Count > 0);
		}

		private void CheckAllGridItemsCheckBox(bool ChechAllItem)
		{
			List<object> selectedValues;
			selectedValues = DynamicHealthGridView.GetSelectedFieldValues(new string[] { "Id" });
			if (selectedValues.Count == 0 && ChechAllItem == true)
			{
				Int32 start = DynamicHealthGridView.VisibleStartIndex;
				Int32 end = DynamicHealthGridView.VisibleStartIndex + DynamicHealthGridView.SettingsPager.PageSize;
				end = (end > DynamicHealthGridView.VisibleRowCount ? DynamicHealthGridView.VisibleRowCount : end);
				for (int i = start; i < end; i++)
				{
					DynamicHealthGridView.Selection.SelectRow(i);
				}
				ASPxCheckBox chkAll = DynamicHealthGridView.FindHeaderTemplateControl(DynamicHealthGridView.Columns[0], "SelectAllCheckBox") as ASPxCheckBox;
				chkAll.Checked = ChechAllItem;
			}
		}


		/// <summary>
		/// This method is used to bind chart control and filter chart control series
		/// </summary>
		private void PerformActions()
		{
			if (ChartTypeComboBox.SelectedIndex != -1 && ShowColumnsComboBox.SelectedIndex != -1)
			{
				switch (_measureType)
				{
					case MeasureTypes.AppHealth:
						//ApplicationHealth
						AppHealthItems displayItems = (AppHealthItems)System.Enum.ToObject(typeof(AppHealthItems), Convert.ToInt32(ShowColumnsComboBox.SelectedItem.Value));
						var applicationHealthSource = (from item in HealthData
																					 let appHealth = item as ApplicationHealth
																					 orderby appHealth.WorkspaceName, appHealth.MeasureDate
																					 let isSelected = GridSelectedRows.Contains(appHealth.CaseArtifactId)
																					 let value = (int?)(displayItems == AppHealthItems.LRQCounts ? appHealth.LRQCount : displayItems == AppHealthItems.ErrorCounts ? appHealth.ErrorCount : displayItems == AppHealthItems.UserCounts ? appHealth.UserCount : 0)
																					 let argumentData = (StartDate == EndDate) ? appHealth.MeasureDate.ToString("hh tt", CultureInfo.InvariantCulture) : appHealth.MeasureDate.Date.ToShortDateString()
																					 select new
																					 {
																						 SeriesData = isSelected ? String.Format("{0} ({1})", appHealth.WorkspaceName, appHealth.CaseArtifactId) : "",
																						 Value = (value < 0 || (!isSelected)) ? null : value,
																						 ArgumentData = argumentData
																					 }).ToList();
						HealthChartControl.DataSource = applicationHealthSource;
						break;
					//Ram Health
					case MeasureTypes.Ram:
						RamHealthItems RamHealthDisplayItems = (RamHealthItems)System.Enum.ToObject(typeof(RamHealthItems), Convert.ToInt32(ShowColumnsComboBox.SelectedItem.Value));

						var ramHealthSource = (from item in HealthData
																	 let ramHealth = item as RamHealth
																	 orderby ramHealth.Server, ramHealth.MeasureDate
																	 let value = (decimal?)Math.Round((RamHealthDisplayItems == RamHealthItems.Page ? ramHealth.PagesPerSec : ramHealth.PageFaultsPerSec), 2)
																	 let argumentData = (StartDate == EndDate) ? ramHealth.MeasureDate.ToString("hh tt", CultureInfo.InvariantCulture) : ramHealth.MeasureDate.Date.ToShortDateString()
																	 let isSelected = GridSelectedRows.Contains(ramHealth.Id)
																	 select new
																	 {
																		 SeriesData = (isSelected ? String.Format("{0} ({1})", ramHealth.Server, ramHealth.ServerType) : ""),
																		 Value = (value < 0 || (!isSelected)) ? null : value,
																		 ArgumentData = argumentData
																	 }).ToList();


						HealthChartControl.DataSource = ramHealthSource;
						break;

					case MeasureTypes.HardDisk:
						//HardDiskHealth
						HardDiskHealthItems HardDiskHealthDisplayItems = (HardDiskHealthItems)System.Enum.ToObject(typeof(HardDiskHealthItems), Convert.ToInt32(ShowColumnsComboBox.SelectedItem.Value));
						var hardDiskHealthSource = (from item in HealthData
																				let hardDiskHealth = item as HardDiskHealth
																				orderby hardDiskHealth.Server, hardDiskHealth.MeasureDate
																				let value = (decimal?)Math.Round((HardDiskHealthDisplayItems == HardDiskHealthItems.DiskRead ? hardDiskHealth.DiskRead : hardDiskHealth.DiskWrite), 2)
																				let argumentData = (StartDate == EndDate) ? hardDiskHealth.MeasureDate.ToString("hh tt", CultureInfo.InvariantCulture) : hardDiskHealth.MeasureDate.Date.ToShortDateString()
																				let isSelected = GridSelectedRows.Contains(hardDiskHealth.Id)
																				select new
																				{
																					SeriesData = isSelected ? hardDiskHealth.ServerDisk : "",
																					Value = (value < 0 || (!isSelected)) ? null : value,
																					ArgumentData = argumentData
																				}).ToList();
						HealthChartControl.DataSource = hardDiskHealthSource;
						break;
					case MeasureTypes.Processor:
						//ProcesserHealth
						ProcesserHealthItems ProcesserHealthDisplayItems = (ProcesserHealthItems)System.Enum.ToObject(typeof(ProcesserHealthItems), Convert.ToInt32(ShowColumnsComboBox.SelectedItem.Value));
						var processerHealthSource = (from item in HealthData
																				 let processerHealth = item as ProcesserHealth
																				 orderby processerHealth.Server, processerHealth.MeasureDate
																				 let value = (decimal?)Math.Round((ProcesserHealthDisplayItems == ProcesserHealthItems.ProcesserTime ? processerHealth.ProcesserTime : 0), 2)
																				 let argumentData = (StartDate == EndDate) ? processerHealth.MeasureDate.ToString("hh tt", CultureInfo.InvariantCulture) : processerHealth.MeasureDate.Date.ToShortDateString()
																				 let isSelected = GridSelectedRows.Contains(processerHealth.Id)
																				 select new
																				 {
																					 SeriesData = isSelected ? String.Format("{0} ({1})", processerHealth.ServerCore, processerHealth.ServerType) : "",
																					 Value = (value < 0 || (!isSelected)) ? null : value,
																					 ArgumentData = argumentData
																				 }).ToList();
						HealthChartControl.DataSource = processerHealthSource;

						break;
					case MeasureTypes.SQLServer:
						//SqlServerHealth
						SqlServerHealthItems SqlServerHealthDisplayItems = (SqlServerHealthItems)System.Enum.ToObject(typeof(SqlServerHealthItems), Convert.ToInt32(ShowColumnsComboBox.SelectedItem.Value));
						var sqlServerHealthSource = (from item in HealthData
																				 let sqlServerHealth = item as SqlServerHealth
																				 orderby sqlServerHealth.Server, sqlServerHealth.MeasureDate
																				 let isSelected = GridSelectedRows.Contains(sqlServerHealth.Id)
																				 let value = (decimal?)Math.Round((SqlServerHealthDisplayItems == SqlServerHealthItems.PageLifeExpectancy ? sqlServerHealth.PageLifeExpectancy : 0), 2)
																				 let argumentData = (StartDate == EndDate) ? sqlServerHealth.MeasureDate.ToString("hh tt", CultureInfo.InvariantCulture) : sqlServerHealth.MeasureDate.Date.ToShortDateString()
																				 select new
																				 {
																					 SeriesData = isSelected ? sqlServerHealth.Server : "",
																					 Value = (value < 0 || (!isSelected)) ? null : value,
																					 ArgumentData = argumentData
																				 }).ToList();
						HealthChartControl.DataSource = sqlServerHealthSource;
						break;
					default:
						break;
				}


				HealthChartControl.SeriesDataMember = "SeriesData";
				HealthChartControl.SeriesTemplate.ArgumentDataMember = "ArgumentData";
				HealthChartControl.SeriesTemplate.ValueDataMembers.AddRange(new string[] { "Value" });
				HealthChartControl.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

				if ((ChartTypes)System.Enum.ToObject(typeof(ChartTypes), Convert.ToInt32(ChartTypeComboBox.SelectedItem.Value)) == ChartTypes.LineChart)
					HealthChartControl.SeriesTemplate.View = new LineSeriesView();
				else
					HealthChartControl.SeriesTemplate.View = new SideBySideBarSeriesView();

				HealthChartControl.DataBind();
				HealthChartControl.EndInit();
				((XYDiagram)HealthChartControl.Diagram).AxisX.Label.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.True;
				((XYDiagram)HealthChartControl.Diagram).AxisX.Label.Angle = -30;
			}


		}

		/// <summary>
		/// Grid Paging Link Button Setting
		/// </summary>
		/// <param name="aspxHyperLink">ASPX Linkbutton</param>
		private void GridPagingLinkButtonSetting(ASPxHyperLink aspxHyperLink)
		{
			aspxHyperLink.ClientSideEvents.Click = string.Join(" ", "function(sender, event) {", string.Format("GetCurrentPageData('{0}');", aspxHyperLink.ID), "}");
		}

		/// <summary>
		/// Show/Hide Filter Link Setting
		/// </summary>
		/// <param name="showFilter">True for show / False for Hide</param>
		private void ShowHideFiltersLinkSetting(bool showFilter)
		{
			if (DynamicHealthGridView.FilterExpression == "")
			{
				ClearAllLink.Enabled = !showFilter;
				ApplyFilterLink.Enabled = !showFilter;
			}
			ShowFiltersLink.ClientSideEvents.Click = "function(sender, event) { __doPostBack('ShowFiltersLink', '" + showFilter + "'); }";
			ClearAllLink.ClientSideEvents.Click = "function(sender, event) { __doPostBack('ClearAllLink', '" + !showFilter + "'); }";
			ApplyFilterLink.ClientSideEvents.Click = "function(sender, event) { __doPostBack('ApplyFilterLink', '" + !showFilter + "'); }";

			UpdateFilterImages(showFilter);
		}

		/// <summary>
		/// Toggle filter images based on Enabled state
		/// </summary>
		/// <param name="showFilter"></param>
		private void UpdateFilterImages(bool showFilter)
		{
			const string imageUrlFormat = "~/Images/{0}.png";

			// change the tooltip to match the image state
			ShowFiltersLink.ToolTip = (showFilter ? "Show Filter" : "Hide Filter");
			ShowFiltersLink.ImageUrl = showFilter
				? String.Format(imageUrlFormat, "itemList_filter")
				: String.Format(imageUrlFormat, "itemList_filter_active");
			// show disabled version of the image if the link is disabled
			ClearAllLink.ImageUrl = ClearAllLink.Enabled
				? String.Format(imageUrlFormat, "clear_filters")
				: String.Format(imageUrlFormat, "clear_filters_disabled");
			ApplyFilterLink.ImageUrl = ApplyFilterLink.Enabled
				? String.Format(imageUrlFormat, "apply_filters")
				: String.Format(imageUrlFormat, "apply_filters_disabled");
		}

		/// <summary>
		/// Show Advance Filter in Grid
		/// </summary>
		/// <param name="showFilter">True for show Advance Filter / False for hide Advance Filter</param>
		private void ShowAdvanceFilterInGrid(bool showFilter)
		{
			FilterGraph();
			DynamicHealthGridView.Settings.ShowFilterRow = showFilter;
			DynamicHealthGridView.Settings.ShowFilterRowMenu = showFilter;
			ShowHideFiltersLinkSetting(!showFilter);
		}

		private void ApplyFilterInGrid(bool applyFilter)
		{
			if (applyFilter == true)
			{
				FilterGraph();
				if (GridSelectedRows != null)
				{
					DynamicHealthGridView.Selection.UnselectAll();

					if (GridSelectedRows.Count > 0)
					{
						for (int i = 0; i < GridSelectedRows.Count; i++)
						{
							DynamicHealthGridView.Selection.SelectRowByKey(Convert.ToInt32(GridSelectedRows[i]));
						}
					}
					foreach (GridViewColumn column in DynamicHealthGridView.Columns)
					{
						if (column.GetType().Name == "GridViewDataColumn")
						{
							GridViewDataColumn dataColumn = column as GridViewDataColumn;
							if (dataColumn.FilterExpression != "")
							{
								dataColumn.HeaderStyle.CssClass = "itemListHeadFilter";
							}
							else
							{
								dataColumn.HeaderStyle.CssClass = "itemListHead";
							}
						}
					}

				}
			}
		}

		private void ClearFilterGrid(bool clearFilter)
		{

			foreach (GridViewColumn column in DynamicHealthGridView.Columns)
			{
				if (column.GetType().Name == "GridViewDataColumn")
				{
					GridViewDataColumn dataColumn = column as GridViewDataColumn;
					dataColumn.HeaderStyle.CssClass = "itemListHead";
				}
			}
			DynamicHealthGridView.FilterExpression = "";
			DynamicHealthGridView.Settings.ShowFilterRow = false;
			DynamicHealthGridView.Settings.ShowFilterRowMenu = false;
			ShowHideFiltersLinkSetting(true);
			FilterGraph();
		}

		protected string GetFailedCount(object failedCount)
		{
			if (this.StartDate == this.EndDate)
				return string.Empty;
			else
				return string.Format("({0})", (int)failedCount);
		}

		protected string GetAnchorClass(object bisInd)
		{
			switch ((BISIndicators)bisInd)
			{
				case BISIndicators.Good:
					return "passed";
				case BISIndicators.Moderate:
					return "probation";
				case BISIndicators.Poor:
					return "failed";
				default:
					return string.Empty;
			}
		}

		protected void ApplicationHealthGridView_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewTableDataCellEventArgs e)
		{
			if (e.DataColumn.FieldName == "BISIndicator")
			{
				var indicators = (BISIndicators)System.Enum.ToObject(typeof(BISIndicators), e.CellValue);
				switch (indicators)
				{
					case BISIndicators.Good:
						e.Cell.BackColor = System.Drawing.Color.Green;
						e.Cell.ForeColor = System.Drawing.Color.White;
						break;
					case BISIndicators.Moderate:
						e.Cell.BackColor = System.Drawing.Color.Yellow;
						break;
					case BISIndicators.Poor:
						e.Cell.BackColor = System.Drawing.Color.Red;
						e.Cell.ForeColor = System.Drawing.Color.White;
						break;
					case BISIndicators.None:
						e.Cell.Text = "&nbsp;";
						break;
					default:
						e.Cell.BackColor = System.Drawing.Color.Black;
						e.Cell.ForeColor = System.Drawing.Color.White;
						break;
				}
			}
		}

		#endregion

		#region IViewHealth Member

		private string HealthDataKey { get { return string.Format("HealthData_{0}", _measureType.ToString()); } }

		/// <summary>
		/// Get application health data from task calling from presenter method
		/// </summary>
		public IList<HealthBase> HealthData
		{
			get
			{
				if (Session[HealthDataKey] == null)
				{
					hdnTimeZoneMinutes.Attributes.Add("UTCDate", DateTime.UtcNow.ToString());
					presenter.GetHealthData();
				}
				return Session[HealthDataKey] as IList<HealthBase>;
			}
			set
			{
				Session[HealthDataKey] = value;
			}
		}
		/// <summary>
		/// Get chart type list from task calling from presenter method
		/// </summary>
		public IList<KeyValue> ChartTypeList
		{
			get;
			set;
		}

		private string ColumnsListKey { get { return string.Format("ColumnsList_{0}", _measureType.ToString()); } }
		/// <summary>
		/// Get column list from task calling from presenter method
		/// </summary>
		public IList<KeyValue> ColumnsList
		{
			get
			{
				if (Session[ColumnsListKey] == null)
				{
					presenter.GetColumnsList();
				}

				return Session[ColumnsListKey] as IList<KeyValue>;
			}
			set
			{
				Session[ColumnsListKey] = value;
			}
		}
		/// <summary>
		/// Get/set start date (used for get  health result)
		/// </summary>
		public DateTime? StartDate { get; set; }
		/// <summary>
		/// Get/set end date (used for get  health result)
		/// </summary>
		/// </summary>
		public DateTime? EndDate { get; set; }

		/// <summary>
		/// Get/set TimeZoneOffset
		/// </summary>
		/// </summary>
		public int TimeZoneOffset { get; set; }

		/// <summary>
		/// Contains selected row values from grid
		/// </summary>
		public List<Int64> GridSelectedRows { get; set; }

		MeasureTypes _measureType = MeasureTypes.AppHealth;
		public MeasureTypes MeasureType
		{
			get
			{
				return _measureType;
			}
		}
		#endregion

		internal class SimpleViewHealth : IViewHealth
		{

			public IList<HealthBase> HealthData { get; set; }

			public IList<KeyValue> ColumnsList { get; set; }

			public DateTime? StartDate { get; set; }

			public DateTime? EndDate { get; set; }

			public int TimeZoneOffset { get; set; }

			public MeasureTypes MeasureType
			{
				get { return MeasureTypes.AppHealth; }
			}
		}

		public void DateRangeIsEuroFormat()
		{
			Boolean usesUkDateFormat = false;

			var cultureDateFormatPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
			cultureDateFormatPattern = cultureDateFormatPattern.ToLower();

			var monthPosition = cultureDateFormatPattern.IndexOf("m");
			var dayPosition = cultureDateFormatPattern.IndexOf("d");

			if (0 <= monthPosition && 0 <= dayPosition)
			{
				if (monthPosition > dayPosition)
				{
					usesUkDateFormat = true;
				}
			}

			Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "useUkDateFormateForDateRangesKey", String.Format("var useUkDateFormateForDateRanges = {0};", usesUkDateFormat.ToString().ToLower()), true);
		}

	}
}
