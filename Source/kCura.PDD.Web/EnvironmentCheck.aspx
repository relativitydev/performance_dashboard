<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnvironmentCheck.aspx.cs" Inherits="kCura.PDD.Web.EnvironmentCheck" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Performance Dashboard - Environment Check</title>
	<script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
	<script type="text/javascript" src="Script/bootstrap.min.js"></script>
	<script type="text/javascript" src="Script/bootstrap-multiselect.js"></script>
	<script type="text/javascript" src="Script/jquery.dataTables.min.js"></script>
	<script type="text/javascript" src="Script/placeholder.jquery.min.js"></script>
	<script type="text/javascript" src="Script/jquery.datetimepicker.js"></script>
	<script type="text/javascript" src="Script/Loading.js"></script>
	<script type="text/javascript" src="Script/DataTablesSetup.js"></script>
	<script type="text/javascript" src="Script/MorelessExpand.js"></script>
	<script type="text/javascript" src="Script/Chart.min.js"></script>
	<script type="text/javascript" src="Script/ScoreChartSetup.js"></script>
	<script type="text/javascript" src="Script/VarscatDetails.js"></script>
	<script type="text/javascript" src="Script/dateformat.js"></script>
	<script type="text/javascript" src="Script/EnvironmentCheck.js"></script>
	<link rel="stylesheet" type="text/css" href="Style/bootstrap.min.css" />
	<link rel="stylesheet" type="text/css" href="Style/bootstrap-multiselect.css" />
	<link rel="stylesheet" type="text/css" href="Style/jquery.datetimepicker.css" />
	<link rel="stylesheet" type="text/css" href="Style/OpenSans.css" />
	<link rel="stylesheet" type="text/css" href="Style/reset.css" />
	<link rel="stylesheet" type="text/css" href="Style/Site.css" />
	<link rel="stylesheet" type="text/css" href="Style/Components.css" />
	<link rel="stylesheet" type="text/css" href="Style/Form.css" />
	<link rel="stylesheet" type="text/css" href="Style/Tables.css" />
	<link rel="stylesheet" type="text/css" href="Style/PageLayout.css" />
	<link rel="stylesheet" type="text/css" href="Style/Loading.css" />
	<link rel="stylesheet" type="text/css" href="Style/UserExperienceSearch.css" />
	<link rel="stylesheet" type="text/css" href="Style/EnvironmentCheck.css" />
</head>
<body>
    <%= System.Web.Helpers.AntiForgery.GetHtml() %>
	<div class="dimmer"></div>
	<div class="loading-pane">
		<div class="loading">
			<img src="Images/loading.gif" />
			<br />
			<strong>Loading...</strong>
		</div>
	</div>
	<div class="action-bar">
		<div class="right" style="text-align: center">
			<a href="javascript:history.go(-1)" type="button" class="btn">Back</a>
			<a href="#" target="_parent" id="QoSNavButton" class="btn" runat="server">QoS Report</a>
			<a href="#" target="_parent" id="EnvCheckServerInfoButton" class="btn" runat="server">Server Information Report</a>
		</div>
		<div class="clear"></div>
	</div>
	<div class="action-bar-secondary pivot-bar">
		<div class="varscat-report-header">
			<h1 id="ReportHeader" runat="server">Environment Check Report</h1>
			<div>
				Environment check information and recommendations for improving your system are detailed below.<br />
				(Items already configured to recommended values are <i>not</i> shown)
			</div>
		</div>
		<div class="manual-run" style="float: right; text-align: right; width: 20px;">
			<div style="text-align: right; float: right; width: 600px;">
				<span id="lblLastRun" style="padding: 5px 10px 5px 10px;">Last Run: --
				</span>
				<button id="btnResetTuningFork" class="btn">Manually Run</button>
			</div>
		</div>
	</div>
	<div class="page-container">
		<div class="table-container table-container-recommendations">
			<div class="container">
				<div class="clear"></div>
				<table id="varscat-table-recommendations" width="100%">
					<thead>
						<tr>
							<th style="min-width: 85px; max-width: 95px">
								<img src="Images/infos.png" height="12" data-placement="right" title="This is an indication of the severity of configuration discrepancy for this setting." />
								Priority
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="right" title="Indicates the server affected by the specified configuration or Relativity if the setting is instance-wide." />
								Scope
							</th>
							<th style="min-width: 85px; max-width: 95px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Indicates the configuration area in which the specified setting can be found." />
								Section
							</th>
							<th style="min-width: 115px">Name
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Description for this setting and its general usage." />
								Description
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Current value in use for the specified configuration. SQL configuration setting changes will take effect after restarting SQL server or executing the RECONFIGURE command." />
								Value
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="kCura's recommendation for changes to this setting." />
								Recommendation
							</th>
						</tr>
						<tr class="searchRow">
							<th>
								<select class="search">
									<option value="">(All)</option>
									<option value="Good">Good</option>
									<option value="Tuning">Tuning</option>
									<option value="Warning">Warning</option>
									<option value="Critical">Critical</option>
									<option value="Not Default">Not Default</option>
								</select>
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
						</tr>
					</thead>
					<tbody>
					</tbody>
				</table>
			</div>
		</div>
	</div>
	<input id="TimezoneOffset" type="hidden" runat="server" />
	<input id="SampleStart" type="hidden" runat="server" />
	<input id="VarscatState" type="hidden" runat="server" />
	<input id="pageServerFilter" type="hidden" />
	<input id="serverSelection" type="hidden" />
	<script type="text/javascript">
		$(document).ready(function () {
			updateSelectedServers();

			var dataGridType = 'recommendations';

			setpage(dataGridType);

			GetLastRunTime(true);

			$('#btnResetTuningFork').click(function () {
                ResetTuningFork(true);
			});

		});
	</script>
</body>
</html>
