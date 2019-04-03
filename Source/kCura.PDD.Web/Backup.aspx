<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Backup.aspx.cs" Inherits="kCura.PDD.Web.RecoverabilityIntegrityReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - Recoverability/Integrity Report</title>
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
            <a href="/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/GapSummary.aspx" class="btn">Backup/DBCC Report</a>
            <a href="/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/RecoveryObjectives.aspx" class="btn">Recovery Objectives Report</a>
            <a href="/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/AdministrationInstall.aspx" class="btn">Reinstall Scripts</a>
            <div class="pivot-controls">
                <img alt="Pivot" class="img-hide-pivot" src="Images/visibility_icon_active.png" />
                <img alt="Pivot" class="img-show-pivot" src="Images/visibility_icon.png" style="display: none" />
            </div>
        </div>
        <div class="clear"></div>
    </div>
    <div class="action-bar-secondary pivot-bar">
        <div id="BestInServiceScoreContainer" runat="server" class="scoreContainer">
            <a href="#" target="_parent" id="QuarterlyScore" runat="server" title="Quarterly Recoverability/Integrity Score" data-placement="bottom"></a>
        </div>
        <div class="varscat-report-header">
            <h1 id="ReportHeader" runat="server">Recoverability/Integrity Report</h1>
            <div>All hours used to determine the recoverability/integrity score are reflected in this report.</div>
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
                            <th>
                                Hour
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="Indicates the time at which recoverability and integrity data was collected and scored." />
                            </th>
                            <th>
                                Overall Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The effective recoverability and integrity score at a given hour based on backup/DBCC history and recovery objectives." />
                            </th>
                            <th>
                                Backup Frequency Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="All databases must have full or differential backups taken at least every ten days. Once this window is exceeded, points are deducted daily based on the largest gap." />
                            </th>
                            <th>
                                Backup Coverage Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="All databases must have full or differential backups taken at least every ten days. Once this window is exceeded, points are deducted based on the percentage of databases in violation." />
                            </th>
                            <th>
                                DBCC Frequency Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="All databases must be checked for corruption at least every ten days. Once this window is exceeded, points are deducted based on the largest gap." />
                            </th>
                            <th>
                                DBCC Coverage Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="All databases must be checked for corruption at least every ten days. Once this window is exceeded, points are deducted based on the percentage of databases in violation." />
                            </th>
                            <th>
                                RPO Score
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="To reduce potential data loss in the event of a disaster or disruption, log backups should be taken frequently. Points are deducted based on maximum data loss over the last week." />
                            </th>
                            <th>
                                RTO Score
                                <img src="Images/infos.png" height="12" data-placement="left" title="Any database should be recoverable within 24 hours in the event of a disaster or disruption. Points are deducted based on the maximum time to recover a single database." />
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
        var state = JSON.parse($("#VarscatState").val());
        varscatDetails.sources.chart = "api/RecoverabilityIntegrity/Scores/";
        varscatDetails.sources.grid = "api/RecoverabilityIntegrityReport/Hours/";
        varscatDetails.sources.csv = "api/RecoverabilityIntegrityReport/GenerateCSV/";
        varscatDetails.emptyTableMessage = "No recoverability/integrity data available with current filters";

        showLoadingAfterTimeout(1000);
        varscatDetails.makeChart();

        var sampleStart = $("#SampleStart").val() + 'Z';
        initializeDatePickers(sampleStart);
        var dt = varscatDetails.makeGrid();
        dt.initDatePickers(sampleStart);

        //Apply filters
        dt.assignFilterValue(0, state.FilterConditions.FriendlySummaryDayHour, state.FilterOperands.SummaryDayHour);
        dt.assignFilterValue(1, state.FilterConditions.RecoverabilityIntegrityScore, state.FilterOperands.RecoverabilityIntegrityScore);
        dt.assignFilterValue(2, state.FilterConditions.BackupFrequencyScore, state.FilterOperands.BackupFrequencyScore);
        dt.assignFilterValue(3, state.FilterConditions.BackupCoverageScore, state.FilterOperands.BackupCoverageScore);
        dt.assignFilterValue(4, state.FilterConditions.DbccFrequencyScore, state.FilterOperands.DbccFrequencyScore);
        dt.assignFilterValue(5, state.FilterConditions.DbccCoverageScore, state.FilterOperands.DbccCoverageScore);
        dt.assignFilterValue(6, state.FilterConditions.RPOScore, state.FilterOperands.RPOScore);
        dt.assignFilterValue(7, state.FilterConditions.RTOScore, state.FilterOperands.RTOScore);

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
        if ($(".scoreChart:visible").length > 0)
            varscatDetails.makeChart();
        var dt = varscatDetails.makeGrid();
        dt.refreshFilterValues();
        Placeholders.enable();

        dt.draw();
    }
</script>
</html>
