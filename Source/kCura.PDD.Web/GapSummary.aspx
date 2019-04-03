﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GapSummary.aspx.cs" Inherits="kCura.PDD.Web.GapSummaryReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - Backup/DBCC Report</title>
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
            <a href="/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/Backup.aspx" class="btn">Recoverability/Integrity Report</a>
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
            <h1 id="ReportHeader" runat="server">Backup/DBCC Report</h1>
            <div>All current gaps in backups and DBCCs and the history of overdue databases are listed below.</div>
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
                            <th>Server</th>
                            <th>Database</th>
                            <th>Activity Type</th>
                            <th>
                                Last Activity
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="The last backup or DBCC - as indicated by the activity type - for this database. Full and differential backups are eligible for gap resolution." />
                            </th>
                            <th>
                                Gap Resolution Date
                                <img src="Images/infos.png" height="12" data-placement="bottom" title="If the gap has been resolved, this is the time of the backup/DBCC." />
                            </th>
                            <th>
                                Gap Size
                                <img src="Images/infos.png" height="12" data-placement="left" title="The number of days that have passed since the last activity." />
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
                                <select class="search">
                                    <option value="">(All)</option>
                                    <option value="Backup">Backup</option>
                                    <option value="DBCC">DBCC</option>
                                </select>
                            </th>
                            <th>
                                <input class="search datetimepicker" type="text" placeholder="(All)" />
                            </th>
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
        varscatDetails.sources.grid = "api/BackupDbccReport/Gaps/";
        varscatDetails.sources.csv = "api/BackupDbccReport/GenerateCSV/";
        varscatDetails.emptyTableMessage = "No backup/DBCC data available with current filters";

        showLoadingAfterTimeout(1000);
        varscatDetails.makeChart();

        var sampleStart = $("#SampleStart").val() + 'Z';
        initializeDatePickers(sampleStart);
        var dt = varscatDetails.makeGrid();
        dt.initDatePickers('1900-01-01T00:00:00Z'); //Gaps might be years old -- we can't set a minimum on these filters

        //Apply filters
        dt.assignFilterValue(0, state.FilterConditions.Server, state.FilterOperands.Server);
        dt.assignFilterValue(1, state.FilterConditions.Database, state.FilterOperands.Database);
        dt.assignFilterValue(2, state.FilterConditions.FriendlyIsBackup, state.FilterOperands.IsBackup);
        dt.assignFilterValue(3, state.FilterConditions.FriendlyLastActivityDate, state.FilterOperands.LastActivityDate);
        dt.assignFilterValue(4, state.FilterConditions.FriendlyResolutionDate, state.FilterOperands.ResolutionDate);
        dt.assignFilterValue(5, state.FilterConditions.GapSize, state.FilterOperands.GapSize);

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
