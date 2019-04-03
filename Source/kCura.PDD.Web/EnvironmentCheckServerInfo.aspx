<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnvironmentCheckServerInfo.aspx.cs" Inherits="kCura.PDD.Web.EnvironmentCheckServerInfo" %>

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
			<a href="#" target="_parent" id="EnvCheckButton" class="btn" runat="server">Environment Check Report</a>
		</div>
		<div class="clear"></div>
	</div>
	<div class="action-bar-secondary pivot-bar">
		<div class="list-toggle-icons">
			<div class="list-toggle-item">
				<div>Server</div>
				<img id="serverShowImgActive" src="Images/list_active_large.png"  />
				<img id="serverShowImgInactive" src="Images/list_inactive_large.png" style="display: none;" />
			</div>
			<div class="list-toggle-item">
				<div>Database</div>
				<img id="databaseShowImgActive" src="Images/list_active_large.png" style="display: none;" />
				<img id="databaseShowImgInactive" src="Images/list_inactive_large.png" />
			</div>
		</div>
		<div class="varscat-report-header">
			<h1 id="ReportHeader" runat="server">Environment Check Server Information Report</h1>
			<div>System details for all monitored servers below.</div>
		</div>
		<div class="manual-run" style="float: right; text-align: right; width: 20px;">
			<div style="text-align: right; float:right; width:600px;">
				<span id="lblLastRun" style="padding: 5px 10px 5px 10px;">Last Run: --
				</span>
				<button id="btnResetTuningFork" class="btn">Manually Run</button>
			</div>
		</div>
	</div>
	<div class="page-container">
		<div class="table-container table-container-server">
			<div class="container">
				<div class="clear"></div>
				<table id="varscat-table-server" width="100%" style="height: 75%;">
					<thead>
						<tr>
							<th style="min-width: 115px">
								Server Name
							</th>
							<th style="min-width: 115px">
								 Operating System Name
							</th>
							<th style="min-width: 115px">
								Operating System Version
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Indicates the number of virtual processors available to the server. If hyperthreading is enabled, this may differ from the count of physical processors." />
								Logical Processors
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Indicates whether hyperthreading is enabled on the specified server, allowing one physical processor to serve as multiple logical processors." />
								Hyperthreaded
							</th>
						</tr>
						<tr class="searchRow">
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
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="0">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<select class="search">
									<option value="">(All)</option>
									<option value="1">True</option>
									<option value="0">False</option>
								</select>
							</th>
						</tr>
					</thead>
					<tbody>
					</tbody>
				</table>
			</div>
		</div>
		<div class="table-container table-container-database">
			<div class="container">
				<div class="clear"></div>
				<table id="varscat-table-database" width="100%" style="height: 75%;">
					<thead>
						<tr>
							<th style="min-width: 115px">
								Server Name
							</th>
							<th style="min-width: 115px">
								SQL Version
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates the current value for the optimize for ad hoc workloads setting. When this option is set, plan cache size is further reduced for single-use ad hoc OLTP workloads." />
								Ad Hoc Workload
							</th>
							<th style="min-width: 135px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Indicates the maximum memory available to SQL server. This may differ from memory available to the operating system based on server configuration." />
								Max Server Memory (GB)
							</th>
							<th style="min-width: 135px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Indicates the current value for the max degree of parallelism option. This limits the number of processors to use in parallel plan execution. When this option is 0, the maximum number of processors is determined by SQL server." />
								Max Degree of Parallelism
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Indicates the number of data files in tempdb on the specified server." />
								TempDB Data Files
							</th>
							<th style="min-width: 115px">
								Last SQL Restart
							</th>
						</tr>
						<tr class="searchRow">
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="0">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="0">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="0">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands datetimepicker" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
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
</body>

<script type="text/javascript">
	$(document).ready(function () {
		updateSelectedServers();

		var dataGridType = 'server';

		setpage(dataGridType);

		GetLastRunTime(false);

        $('#btnResetTuningFork').click(function () {
            ResetTuningFork(false);
		});

		$('#recommendationsShowImgInactive').click(function () {
			setActiveListImages('recommendations');
			setpage('recommendations');
		});

		$('#serverShowImgInactive').click(function () {
			setActiveListImages('server');
			setpage('server');
		});

		$('#databaseShowImgInactive').click(function () {
			setActiveListImages('database');
			setpage('database');
		});
    });

	function setActiveListImages(activeType) {
		var types = ['server', 'database'];
		for (var i = 0; i < types.length; i++) {
			if (types[i] == activeType) {
				$('#' + types[i] + 'ShowImgActive').show();
				$('#' + types[i] + 'ShowImgInactive').hide();
			} else {
				$('#' + types[i] + 'ShowImgActive').hide();
				$('#' + types[i] + 'ShowImgInactive').show();
			}
		}
	}

	
</script>
</html>
