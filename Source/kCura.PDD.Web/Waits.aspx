<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Waits.aspx.cs" Inherits="kCura.PDD.Web.Waits" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Performance Dashboard - SQL Waits Report</title>
	<script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
	<script type="text/javascript" src="Script/bootstrap.min.js"></script>
	<script type="text/javascript" src="Script/bootstrap-multiselect.js"></script>
	<script type="text/javascript" src="Script/jquery.dataTables.min.js"></script>
	<script type="text/javascript" src="Script/placeholder.jquery.min.js"></script>
	<script type="text/javascript" src="Script/jquery.datetimepicker.js"></script>
	<script type="text/javascript" src="Script/Loading.js"></script>
	<script type="text/javascript" src="Script/DataTablesSetup.js"></script>
	<script type="text/javascript" src="Script/Chart.min.js"></script>
	<script type="text/javascript" src="Script/ScoreChartSetup.js"></script>
	<script type="text/javascript" src="Script/VarscatDetails.js"></script>
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
			<a href="#" target="_parent" id="BtnFileLatency" class="btn" runat="server">File Latency Report</a>
			<div class="pivot-controls">
				<img alt="Pivot" class="img-hide-pivot" src="Images/visibility_icon_active.png" />
				<img alt="Pivot" class="img-show-pivot" src="Images/visibility_icon.png" style="display: none" />
			</div>
		</div>
		<div class="clear"></div>
	</div>
	<div class="action-bar-secondary pivot-bar">
		<div id="BestInServiceScoreContainer" runat="server" class="scoreContainer">
			<a href="#" target="_parent" id="QuarterlyScore" runat="server" title="Quarterly Infrastructure Performance Score" data-placement="bottom"></a>
		</div>
		<div class="varscat-report-header">
			<h1 id="ReportHeader" runat="server">SQL Waits Report</h1>
			<div>All hours used to determine the current infrastructure performance scores are reflected in this report.</div>
		</div>
		<div class="pivot-toggle-icons">
			<img class="img-hide-chart" src="Images/pivot_chart_active_large.png" />
			<img class="img-show-chart" src="Images/pivot_chart_inactive_large.png" style="display: none" />
			<img class="img-hide-grid" src="Images/pivot_list_active_large.png" style="margin-left: 8px" />
			<img class="img-show-grid" src="Images/pivot_list_inactive_large.png" style="margin-left: 8px; display: none" />
		</div>
		<div class="page-date-filters">
			<div>
				<label for="startDate">Start Date</label>
				<input class="search datepicker startdatepicker" id="startDate" runat="server" type="text" />
				<div class="btn updatePageLevelFilters" style="float: right">Update</div>
			</div>
			<div style="margin-top: 3px">
				<label for="endDate">End Date</label>
				<input class="search datepicker enddatepicker" id="endDate" runat="server" type="text" />
				<div class="btn resetPageLevelFilters" style="float: right">Reset</div>
			</div>
		</div>
		<div class="page-server-filters">
			<label for="pageServerSelect">Server</label>
			<select id="pageServerSelect" runat="server" multiple="true">
				<option value="">(All)</option>
			</select>
		</div>
	</div>
	<div class="page-container">
		<div class="action-bar-secondary chart-bar">
			<div class="legend"></div>
			<div class="dataTables_wrapper containsScoreChart">
				<canvas id="scoreChart" class="scoreChart" height="250" width="800"></canvas>
			</div>
		</div>
		<div class="table-container">
			<div class="container">
				<div class="clear"></div>
				<table id="varscat-table" width="100%">
					<thead>
						<tr>
							<th style="min-width: 115px">Hour
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Aggregates are based on actions taken in the hour following the time indicated here (expressed in your local time zone)" />
							</th>
							<th>Server</th>
							<th>Waits Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The waits component of the SQL server score for a given hour, determined based on signal waits ratio and presence of poison waits." /></th>
							<th>Signal Waits Ratio
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The ratio of signal to resource waits for a given hour." /></th>
							<th>Processor Time Utilization
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates the ratio of total wait time to available processor time for the hour. When this ratio is 0.75 or higher, a high signal-to-resource waits ratio will impact the score." /></th>
							<th>Wait Type
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The name of a resource SQL is waiting for." /></th>
							<th>Signal Wait Time (ms)
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The amount of time waiting on the runnable queue (waiting for CPU)." /></th>
							<th>Wait Time (ms)
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The overall wait time for this type (including signal wait time)." /></th>
							<th>Waiting Task Count
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates the number of threads waiting on this resource in the hour. SOS_SCHEDULER_YIELD waits are excluded from the signal-to-resource waits ratio and scoring if their waiting tasks' average wait time is less than 1ms. This is true when the waiting task count exceeds the wait time." /></th>
							<th>Poison Wait
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates whether the given wait type is classified as a poison wait. The presence of any poison wait for more than 1000ms in an hour will result in a waits score of 0." /></th>
							<th>Weekly Sample
                                <img src="Images/infos.png" height="12" data-placement="left" title="Indicates whether the given hour is included in the weekly sample set." /></th>
						</tr>
						<tr class="searchRow">
							<th>
								<input class="search datetimepicker" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<input class="search supports-operands" type="text" placeholder="(All)" />
								<select class="operand">
									<option value="">=</option>
									<option value="1"><</option>
									<option value="2">></option>
									<option value="3"><=</option>
									<option value="4">>=</option>
								</select>
							</th>
							<th>
								<select class="search">
									<option value="">(All)</option>
									<option value="Yes">Yes</option>
									<option value="No">No</option>
								</select>
							</th>
							<th>
								<select class="search">
									<option value="">(All)</option>
									<option value="Yes">Yes</option>
									<option value="No">No</option>
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
	<input id="VarscatState" type="hidden" runat="server" />
	<input id="TimezoneOffset" type="hidden" runat="server" />
	<input id="SampleStart" type="hidden" runat="server" />
	<input id="pageServerFilter" type="hidden" />
	<input id="serverSelection" type="hidden" />
	<input id="DateFormatString" type="hidden" runat="server" />
	<input id="TimeFormatString" type="hidden" runat="server" />
</body>

<script type="text/javascript">
	$(document).ready(function () {
		updateSelectedServers();

		var state = JSON.parse($("#VarscatState").val());
		varscatDetails.sources.chart = "api/SystemLoad/Scores/";
		varscatDetails.sources.grid = "api/SystemLoadWaits/Waits/";
		varscatDetails.sources.csv = "api/SystemLoadWaits/GenerateCSV/";

		showLoadingAfterTimeout(1000);
		varscatDetails.makeServerSelect();
		varscatDetails.makeChart();

		var sampleStart = $("#SampleStart").val() + 'Z';
		initializeDatePickers(sampleStart);
		var dt = varscatDetails.makeGrid();
		dt.initDatePickers(sampleStart);

		//Apply filters
		dt.assignFilterValue(0, state.FilterConditions.FriendlySummaryDayHour, state.FilterOperands.SummaryDayHour);
		dt.assignFilterValue(1, state.FilterConditions.Server, state.FilterOperands.Server);
		dt.assignFilterValue(2, state.FilterConditions.OverallScore, state.FilterOperands.OverallScore);
		dt.assignFilterValue(3, state.FilterConditions.SignalWaitsRatio, state.FilterOperands.SignalWaitsRatio);
		dt.assignFilterValue(4, state.FilterConditions.WaitType, state.FilterOperands.WaitType);
		dt.assignFilterValue(5, state.FilterConditions.SignalWaitTime, state.FilterOperands.SignalWaitTime);
		dt.assignFilterValue(6, state.FilterConditions.TotalWaitTime, state.FilterOperands.TotalWaitTime);
		dt.assignFilterValue(7, state.FilterConditions.FriendlyIsPoisonWait, state.FilterOperands.FriendlyIsPoisonWait);
		dt.assignFilterValue(8, state.FilterConditions.FriendlyIsActiveWeeklySample, state.FilterOperands.IsActiveWeeklySample);

		dt.assignFilterValue(9, state.FilterConditions.PercentOfCPUThreshold, state.FilterOperands.IsActiveWeeklySample);
		dt.assignFilterValue(10, state.FilterConditions.FriendlyIsActiveWeeklySample, state.FilterOperands.IsActiveWeeklySample);

		dt.assignSort(state.GridConditions.SortIndex, state.GridConditions.SortDirection);
		dt.assignStartIndex(state.GridConditions.StartRow - 1);

		//Bind events
		dt.on('xhr', hideLoading);
		dt.on('preXhr', showLoadingAfterTimeout(1000));
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
	});

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
</script>
</html>
