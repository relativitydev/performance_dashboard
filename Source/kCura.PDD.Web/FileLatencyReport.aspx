<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileLatencyReport.aspx.cs" Inherits="kCura.PDD.Web.FileLatencyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Performance Dashboard - File Latency Report</title>
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
</head>
<body>
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
			<a href="#" target="_parent" id="BtnServer" class="btn" runat="server">Server Report</a>
			<a href="#" target="_parent" id="BtnWaits" class="btn" runat="server">Waits Report</a>
		</div>
		<div class="clear"></div>
	</div>
	<div class="action-bar-secondary pivot-bar">
		<div class="varscat-report-header">
			<h1 id="ReportHeader" runat="server">File Latency Report</h1>
			<div>Data and log file latency statistics for Relativity databases considered in scoring are detailed below. Only the most recent hour's statistics are shown.</div>
		</div>
		<div class="manual-run" style="float: right; text-align: right; width: 20px;">
			<div style="text-align: right; float:right; width:600px;">
				<span id="lblLastRun" style="padding: 5px 10px 5px 10px;">Last Run: --
				</span>
			</div>
		</div>
	</div>
	<div class="page-container">
		<div class="table-container table-container-filelatencyreport">
			<div class="container">
				<div class="clear"></div>
				<table id="varscat-table" width="100%">
					<thead>
						<tr>
							<th style="min-width: 115px;">
								Server Name
							</th>
							<th style="min-width: 115px;">
								Database Name
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Effective latency score for the given database in the most recent hour. The score is based on data file read/write latency and average log file write latency." />
								Score
							</th>
							<th style="min-width: 115px;">
								<img src="Images/infos.png" height="12" data-placement="left" title="Average latency of read operations on data files for the given database over the most recent hour (expressed in milliseconds). The data file with the highest overall latency is used." />
								Data Read Latency
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Average latency of write operations on data files for the given database over the most recent hour (expressed in milliseconds). The data file with the highest overall latency is used." />
								Data Write Latency
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Average latency of read operations on log files for the given database over the most recent hour (expressed in milliseconds). The log file with the highest overall latency is used." />
								Log Read Latency
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Average latency of write operations on log files for the given database over the most recent hour (expressed in milliseconds). The log file with the highest overall latency is used." />
								Log Write Latency
							</th>
						</tr>
						<tr class="searchRow">
							<th>
								<input class="search" type="text" placeholder="(All)"  />
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
		//updateSelectedServers();

		setpage();

		GetLastRunTime();

		//$('#btnResetFileLatency').click(function () {
		//	$('#btnResetFileLatency').prop("disabled", true);
		//	$.post('api/FileLatency/ResetFileLatencyLastRun', function (response) {
		//		SetLastRunTime(response);
		//		$('#btnResetFileLatency').prop("disabled", false);
		//	});
		//});

	});

	function setpage() {

		var state = JSON.parse($("#VarscatState").val());

		varscatDetails.sources.grid = 'api/FileLatency/GetDatabaseLatencyReport';
		varscatDetails.sources.csv = 'api/FileLatency/GenerateCSV/';
		varscatDetails.emptyTableMessage = "No file latency report information available with current filters";

		showLoadingAfterTimeout(1000);
		varscatDetails.makeServerSelect();

		var dt = varscatDetails.makeGridTable('#varscat-table',
			function () {
				$('#initialServerName').val('');
			}
			, null, { initialServerName: state.FilterConditions.ServerName });

		//Bind events
		dt.on('xhr', hideLoading);
		dt.on('preXhr', showLoadingAfterTimeout(1000));

		dt.initDatePickers('1900-01-01T00:00:00Z');

		dt.assignFilterValue(0, state.FilterConditions.ServerName, state.FilterOperands.ServerName);

		$(".resetPageLevelFilters").click(function () {
			$("#pageServerSelect option:selected").each(function () {
				$(this).prop('selected', false);
			});
			$("#pageServerSelect").multiselect('refresh');
			$(".datepicker").datetimepicker('reset');
			updatePageFilters();
		});

		//Grab the data
		dt.draw(false);
	}


	function updatePageFilters() {
		updateSelectedServers();

		if ($(".scoreChart:visible").length > 0)
			varscatDetails.makeChart();
		var dt = varscatDetails.makeGrid();
		dt.refreshFilterValues();
		var pageServerId = $("#serverSelection").val();
		dt.assignFilterValue(1, pageServerId.split(',').length == 1 ? pageServerId : '');
		Placeholders.enable();

		dt.draw();
	}

	function GetLastRunTime() {
		$.get('api/FileLatency/FileLatencyLastRun', function (response) {
			SetLastRunTime(response);
			setTimeout(GetLastRunTime, 1000 * 20);
		});
	}

	function SetLastRunTime(lastRun) {
		var date = new Date(Date.parse(lastRun + 'Z'));
		if (date.getFullYear() == 1900) {
			$("#lblLastRun").html('Last Run: Pending');
		} else {
			var dateStr = date.toLocaleString();
			$('#lblLastRun').html('Last Run: ' + dateStr);
		}
	}

</script>
</html>
