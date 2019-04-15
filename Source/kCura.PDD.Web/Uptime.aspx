<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Uptime.aspx.cs" Inherits="kCura.PDD.Web.Uptime" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Performance Dashboard - Uptime Details</title>
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
            <div class="pivot-controls">
                <img alt="Pivot" class="img-hide-pivot" src="Images/visibility_icon_active.png" />
                <img alt="Pivot" class="img-show-pivot" src="Images/visibility_icon.png" style="display: none" />
            </div>
        </div>
        <div class="clear"></div>
    </div>
    <div class="action-bar-secondary pivot-bar">
        <div id="BestInServiceScoreContainer" runat="server" class="scoreContainer">
            <a href="#" target="_parent" id="QuarterlyScore" runat="server" title="Quarterly User Experience" data-placement="bottom"></a>
        </div>
        <div class="varscat-report-header">
            <h1 id="ReportHeader" runat="server">Uptime Report</h1>
            <div>Uptime history over the last 90 days is detailed below.</div>
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
                <table id="varscat-table" width="100%" style="height: 75%">
                    <thead>
                        <tr>
                            <th style="min-width: 115px">
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Uptime and scoring information are calculated for the hour shown (expressed in your local time zone)." />
                                Hour
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The environment uptime score for a given hour. This is based exclusively on the uptime percentage for that hour." />
                                Score
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates the status of Relativity in a given hour. When web, SQL, or agent downtime occurs, the server type with the highest impact is listed." />
                                Status
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="left" title="The percentage of uptime for a given hour. This is impacted when web servers are inaccessible or agent check-ins are interrupted due to SQL/agent server downtime." />
                                Uptime
                            </th>
                        </tr>
                        <tr class="searchRow">
                            <th>
                                <input class="search datetimepicker" type="text" placeholder="(All)" />
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
                                    <option value="Accessible">Accessible</option>
                                    <option value="All Web Servers Down">All Web Servers Down</option>
                                    <option value="SQL/Agent Servers Down">SQL/Agent Servers Down</option>
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
    <input id="DateFormatString" type="hidden" runat="server" />
    <input id="TimeFormatString" type="hidden" runat="server" />
</body>

<script type="text/javascript">
    $(document).ready(function () {
        updateSelectedServers();

        var state = JSON.parse($("#VarscatState").val());
        varscatDetails.sources.chart = "api/Uptime/Scores/";
        varscatDetails.sources.grid = "api/UptimeReport/Hours/";
        varscatDetails.sources.csv = "api/UptimeReport/GenerateCSV/";

        showLoadingAfterTimeout(1000);
        varscatDetails.makeServerSelect();
        varscatDetails.makeChart();

        var sampleStart = $("#SampleStart").val() + 'Z';
        initializeDatePickers(sampleStart);
        var dt = varscatDetails.makeGrid();
        dt.initDatePickers(sampleStart);

        //Assign filter/sort/page state
        dt.assignFilterValue(0, state.FilterConditions.FriendlySummaryDayHour, state.FilterOperands.SummaryDayHour);
        dt.assignFilterValue(1, state.FilterConditions.Score, state.FilterOperands.Score);
        dt.assignFilterValue(2, state.FilterConditions.Status, state.FilterOperands.Status);
        dt.assignFilterValue(3, state.FilterConditions.Uptime, state.FilterOperands.Uptime);

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
