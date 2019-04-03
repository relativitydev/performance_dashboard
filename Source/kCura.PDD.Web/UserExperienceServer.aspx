<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserExperienceServer.aspx.cs" Inherits="kCura.PDD.Web.UserExperienceServers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - User Experience Server Details</title>
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
            <h1 id="ReportHeader" runat="server">User Experience Report</h1>
            <div>All hours used to determine the current user experience scores are reflected in this report.</div>
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
                <table id="varscat-table" width="100%" style="height: 75%">
                    <thead>
                        <tr>
                            <th style="min-width: 115px">
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Aggregates are based on actions taken in the hour following the time indicated here (expressed in your local time zone)" />
                                Hour
                            </th>
                            <th>Server</th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The server's user experience score for a given hour, determined based on the number of active users and percentage of simple document searches that took longer than two seconds." />
                                Score
                            </th>
                            <th>Workspace</th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The number of document searches exceeding the long-running threshold (two seconds for simple searches, eight seconds for complex)" />
                                Long-Running Queries
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The distinct number of users in the workspace for a given hour" />
                                Total Users
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The number of document search audits collected by VARSCAT (includes TOP and COUNT audits)" />
                                Total Search Audits
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The number of audits collected by VARSCAT of types other than document search" />
                                Total Non-Search Audits
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The number of Relativity audits collected by VARSCAT" />
                                Total Audits
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The sum of execution time for all actions expressed in milliseconds" />
                                Total Execution Time
                            </th>
                            <th>
                                <img src="Images/infos.png" height="12" data-placement="left" title="Indicates whether the given hour was included in the weekly sample set" />
                                Weekly Sample
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
    <input id="TimezoneOffset" type="hidden" runat="server" />
    <input id="SampleStart" type="hidden" runat="server" />
    <input id="VarscatState" type="hidden" runat="server" />
    <input id="DateFormatString" type="hidden" runat="server" />
    <input id="TimeFormatString" type="hidden" runat="server" />
    <input id="pageServerFilter" type="hidden" />
    <input id="serverSelection" type="hidden" />
</body>

<script type="text/javascript">
    $(document).ready(function () {
        updateSelectedServers();

        var state = JSON.parse($("#VarscatState").val());
        varscatDetails.sources.chart = "api/UserExperience/Scores/";
        varscatDetails.sources.grid = "api/UserExperienceServers/Servers/";
        varscatDetails.sources.csv = "api/UserExperienceServers/GenerateCSV/";

        showLoadingAfterTimeout(1000);
        varscatDetails.makeServerSelect();
        varscatDetails.makeChart();

        var sampleStart = $("#SampleStart").val() + 'Z';
        initializeDatePickers(sampleStart);
        var dt = varscatDetails.makeGrid();
        dt.initDatePickers(sampleStart);

        //Assign filter/sort/page state
        dt.assignFilterValue(0, state.FilterConditions.FriendlySummaryDayHour, state.FilterOperands.SummaryDayHour);
        dt.assignFilterValue(1, state.FilterConditions.Server, state.FilterOperands.Server);
        dt.assignFilterValue(2, state.FilterConditions.Score, state.FilterOperands.Score);
        dt.assignFilterValue(3, state.FilterConditions.Workspace, state.FilterOperands.Workspace);
        dt.assignFilterValue(4, state.FilterConditions.TotalLongRunning, state.FilterOperands.TotalLongRunning);
        dt.assignFilterValue(5, state.FilterConditions.TotalUsers, state.FilterOperands.TotalUsers);
        dt.assignFilterValue(6, state.FilterConditions.TotalSearchAudits, state.FilterOperands.TotalSearchAudits);
        dt.assignFilterValue(7, state.FilterConditions.TotalNonSearchAudits, state.FilterOperands.TotalNonSearchAudits);
        dt.assignFilterValue(8, state.FilterConditions.TotalAudits, state.FilterOperands.TotalAudits);
        dt.assignFilterValue(9, state.FilterConditions.TotalExecutionTime, state.FilterOperands.TotalExecutionTime);
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
