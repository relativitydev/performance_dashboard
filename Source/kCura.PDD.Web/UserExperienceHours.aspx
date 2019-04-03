<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserExperienceHours.aspx.cs" Inherits="kCura.PDD.Web.UserExperienceHours" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - User Experience Hour Details</title>
    <script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="Script/bootstrap.min.js"></script>
    <script type="text/javascript" src="Script/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="Script/placeholder.jquery.min.js"></script>
    <script type="text/javascript" src="Script/jquery.datetimepicker.js"></script>
    <script type="text/javascript" src="Script/Loading.js"></script>
    <script type="text/javascript" src="Script/DataTablesSetup.js"></script>
    <script type="text/javascript" src="Script/Chart.min.js"></script>
    <script type="text/javascript" src="Script/ScoreChartSetup.js"></script>
    <script type="text/javascript" src="Script/VarscatDetails.js"></script>
    <link rel="stylesheet" type="text/css" href="Style/bootstrap.min.css" />
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
            <h1 id="ReportHeader" runat="server">Hours View Report</h1>
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
                <table id="varscat-table" width="100%">
                    <thead>
                        <tr>
                            <th style="min-width: 115px">Hour
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Aggregates are based on actions taken in the hour starting at the time indicated here (expressed in your local time zone)" />
                            </th>
                            <th>Database</th>
                            <th>Search</th>
                            <th>Complex / Simple
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates whether the search was classified as simple or complex based on its conditions" /></th>
                            <th>Total Run Time
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The total execution time of all runs of a search in the given hour expressed in milliseconds" /></th>
                            <th>Average Run Time
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The average execution time of all runs of a search in the given hour expressed in milliseconds" /></th>
                            <th>Total Search Audits
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The number of audited queries associated with this search in the given hour" /></th>
                            <th>Weekly Sample
                                <img src="Images/infos.png" height="12" data-placement="left" title="Indicates whether the given hour was included in the weekly sample set" /></th>
                        </tr>
                        <tr class="searchRow">
                            <th>
                                <input class="search datetimepicker" type="text" placeholder="(All)" />
                            </th>
                            <th>
                                <input class="search" type="text" placeholder="(All)" />
                            </th>
                            <th>
                                <input class="search" type="text" placeholder="(All)" />
                            </th>
                            <th>
                                <select class="search">
                                    <option value="">(All)</option>
                                    <option value="Complex">Complex</option>
                                    <option value="Simple">Simple</option>
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
    <input id="serverSelection" type="hidden" />
    <input id="TimezoneOffset" type="hidden" runat="server" />
    <input id="SampleStart" type="hidden" runat="server" />
    <input id="VarscatState" type="hidden" runat="server" />
    <input id="DateFormatString" type="hidden" runat="server" />
    <input id="TimeFormatString" type="hidden" runat="server" />
</body>

<script type="text/javascript">
    $(document).ready(function () {
        var state = JSON.parse($("#VarscatState").val());
        varscatDetails.sources.chart = "api/UserExperience/Scores/";
        varscatDetails.sources.grid = "api/UserExperienceHours/Hours/" + state.FilterConditions.Server;
        varscatDetails.sources.csv = "api/UserExperienceHours/GenerateCSV/" + state.FilterConditions.Server;
        $("#serverSelection").val(state.FilterConditions.Server || -1);

        showLoadingAfterTimeout(1000);
        varscatDetails.makeChart();

        var offset = $("#TimezoneOffset").val() || '0';
        var sampleStart = $("#SampleStart").val() + 'Z';
        initializeDatePickers(sampleStart);
        var dt = varscatDetails.makeGrid();
        dt.initDatePickers(sampleStart);

        //Assign filters
        dt.assignFilterValue(0, state.FilterConditions.FriendlySummaryDayHour, state.FilterOperands.SummaryDayHour);
        dt.assignFilterValue(1, state.FilterConditions.Workspace, state.FilterOperands.Workspace);
        dt.assignFilterValue(2, state.FilterConditions.Search, state.FilterOperands.Search);
        dt.assignFilterValue(3, state.FilterConditions.FriendlyIsComplex, state.FilterOperands.IsComplex);
        dt.assignFilterValue(4, state.FilterConditions.TotalRunTime, state.FilterOperands.TotalRunTime);
        dt.assignFilterValue(5, state.FilterConditions.AverageRunTime, state.FilterOperands.AverageRunTime);
        dt.assignFilterValue(6, state.FilterConditions.NumberOfRuns, state.FilterOperands.TotalRuns);
        dt.assignFilterValue(7, state.FilterConditions.FriendlyIsActiveWeeklySample, state.FilterOperands.IsActiveWeeklySample);

        dt.assignSort(state.GridConditions.SortIndex, state.GridConditions.SortDirection);
        dt.assignStartIndex(state.GridConditions.StartRow - 1);

        //Bind events
        dt.on('xhr', hideLoading);
        dt.on('preXhr', showLoadingAfterTimeout(1000));
        $(".resetPageLevelFilters").click(function () {
            $(".datepicker").datetimepicker('reset');
            updatePageFilters();
        });

        //Grab the data
        dt.draw(false);
    });

    function updatePageFilters() {
        varscatDetails.makeChart();
        var dt = varscatDetails.makeGrid();
        dt.refreshFilterValues();
        dt.draw();
    }
</script>
</html>
