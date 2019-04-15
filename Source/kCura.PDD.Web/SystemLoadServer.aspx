<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemLoadServer.aspx.cs" Inherits="kCura.PDD.Web.SystemLoadServers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - Infrastructure Performance Details</title>
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
			<a href="#" target="_parent" id="BtnWaits" class="btn" runat="server">Waits Report</a>
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
            <h1 id="ReportHeader" runat="server">Infrastructure Performance Report</h1>
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
                            <th style="min-width: 115px">
                                Hour
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Aggregates are based on actions taken in the hour following the time indicated here (expressed in your local time zone)" />
                            </th>
                            <th>Server</th>
                            <th>Server Type</th>
                            <th>
                                Overall Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The server's infrastructure performance score for a given hour, determined based on CPU/RAM utilization for all server types and SQL server memory signals, wait statistics, and file-level latency." />
                            </th>
                            <th>
                                CPU Utilization Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The server's average CPU utilization should be less than 60%. Points are deducted for higher utilization with a maximum deduction at 85%. This carries a weight of 22.5% for SQL servers and 50% for web servers." />
                            </th>
                            <th>
                                RAM Utilization Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Web servers should have at least 1 GB of free memory on average. For SQL servers, at least 4 GB should be free. Points are deducted on a logarithmic scale for higher utilization. This carries a weight of 22.5% for SQL servers and 50% for web servers." />
                            </th>
                            <th>
                                SQL Memory Signal Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="SQL servers are scored based on memory signal state and pageouts. Points are deducted based on the frequency of memory pressure. Pageouts during low memory periods will result in the loss of all points. This carries a weight of 22.5% for SQL servers." />
                            </th>
                            <th>
                                SQL Waits Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="SQL servers are scored based on the ratio of signal to resource waits and the presence of poison waits. The signal waits ratio should not exceed 10%. The presence of any poison wait type for one second within an hour will result in an overall score of 0 for that server and hour. This carries a weight of 22.5% for SQL servers." />
                            </th>
                            <th>
                                Virtual Log Files Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="SQL servers are scored based on the maximum number of virtual log files across all databases. These should be kept to 10000 or fewer. This carries a weight of 5% for SQL servers." />
                            </th>
                            <th>
                                Latency Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="SQL servers are scored based on file-level latency when PAGEIOLATCH_% waits are high. Read latency for data files should not exceed 100ms. Write latency for data and log files should not exceed 30ms and 10ms, respectively. This carries a weight of 5% for SQL servers." />
                            </th>
                            <th>
                                Weekly Sample
                                <img src="Images/infos.png" height="12" data-placement="left" title="Indicates whether the given hour is included in the weekly sample set." />
                            </th>
                        </tr>
                        <tr class="searchRow">
                            <th>
                                <input class="search datetimepicker" type="text" placeholder="(All)" />
                            </th>
                            <th>
                                <input class="search" type="text" placeholder="(All)" />
                            </th>
                            <th>
                                <select class="search">
                                    <option value="">(All)</option>
                                    <option value="SQL">SQL</option>
                                    <option value="Web">Web</option>
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
        varscatDetails.sources.grid = "api/SystemLoadServers/Servers/";
        varscatDetails.sources.csv = "api/SystemLoadServers/GenerateCSV/";

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
        dt.assignFilterValue(2, state.FilterConditions.ServerType, state.FilterOperands.ServerType);
        dt.assignFilterValue(3, state.FilterConditions.OverallScore, state.FilterOperands.OverallScore);
        dt.assignFilterValue(4, state.FilterConditions.CPUScore, state.FilterOperands.CPUUtilizationScore);
        dt.assignFilterValue(5, state.FilterConditions.RAMScore, state.FilterOperands.RAMUtilizationScore);
        dt.assignFilterValue(6, state.FilterConditions.MemorySignalScore, state.FilterOperands.MemorySignalScore);
        dt.assignFilterValue(7, state.FilterConditions.WaitsScore, state.FilterOperands.WaitsScore);
        dt.assignFilterValue(8, state.FilterConditions.VirtualLogFilesScore, state.FilterOperands.VirtualLogFilesScore);
        dt.assignFilterValue(9, state.FilterConditions.LatencyScore, state.FilterOperands.LatencyScore);
        dt.assignFilterValue(10, state.FilterConditions.IsActiveWeeklySample, state.FilterOperands.IsActiveWeeklySample);

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
