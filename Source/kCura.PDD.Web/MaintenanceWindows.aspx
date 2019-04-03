<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaintenanceWindows.aspx.cs" Inherits="kCura.PDD.Web.MaintenanceWindows" %>
<% var isSystemAdmin = new kCura.PDD.Web.Factories.AuthenticationServiceFactory().GetService().IsSystemAdministrator();  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Performance Dashboard - Maintenance Windows</title>
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
	<script type="text/javascript" src="Script/MaintenanceWindows.js"></script>
    
    <link rel="stylesheet" type="text/css" href="Fonts/font.relativity-user-permissions-icons.css"/>
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
            <% if ( isSystemAdmin ) { %>
                <a href="#" target="_parent" id="CreateMWButton" class="btn" runat="server">Create New Maintenance Window</a>
            <% } %>
			<a href="#" target="_parent" id="QoSNavButton" class="btn" runat="server">QoS Report</a>
		</div>
		<div class="clear"></div>
	</div>
	<div class="action-bar-secondary pivot-bar">
		<div class="varscat-report-header">
			<h1 id="ReportHeader" runat="server">Maintenance Windows</h1>
			<div>
				All scheduled maintenance windows appear in this list.  Maintenance windows can be cancelled prior to their start period.  All maintenance windows must be scheduled at least 48 hours in advance.
			</div>
		</div>
	</div>
	<div class="page-container">
		<div class="table-container table-container-MaintenanceWindows">
			<div class="container">
				<div class="clear"></div>
				<table id="varscat-table-MaintenanceWindows" width="100%">
					<thead>
						<tr>
							<th style="min-width: 24px; width: 40px;">
								<% if ( isSystemAdmin ) { %>Remove<% } %>
							</th>
							<th style="min-width: 115px;">
								<img src="Images/infos.png" height="12" data-placement="right" title="The start time of the scheduled maintenance window in UTC." />
								Start Period
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="right" title="The end time of the scheduled maintenance window in UTC." />
								End Period
							</th>
							<th style="min-width: 85px; max-width: 95px">
								<img src="Images/infos.png" height="12" data-placement="left" title="The reason given for the scheduled maintenance window." />
								Reason
							</th>
							<th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Additional information regarding the maintenance window." />
								Comments
							</th>
                            <th style="min-width: 115px">
								<img src="Images/infos.png" height="12" data-placement="left" title="Duration of the scheduled maintenance window in hours." />
								Duration
							</th>

						</tr>
						<tr class="searchRow">
							<th>

							</th>
							<th>
								<input class="search datetimepicker supports-operands" type="text" placeholder="(All)" />
                                <select class="operand">
                                    <option value="">=</option>
                                    <option value="1"><</option>
                                    <option value="2">></option>
                                    <option value="3"><=</option>
                                    <option value="4">>=</option>
                                </select>
							</th>
							<th>
								<input class="search datetimepicker supports-operands" type="text" placeholder="(All)" />
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
							        <!-- Mirrors MaintenanceWindowReason enum displaynames and int values -->
									<option value="">(All)</option>
									<option value="2">Hardware Migration</option>
                                    <option value="1">Hardware Upgrade</option>
									<option value="3">Network Maintenance</option>
                                    <option value="7">Operating System Updates</option>
                                    <option value="8">Other (see comments)</option>
									<option value="4">Relativity Upgrade (major release)</option>
									<option value="5">Relativity Upgrade (patch)</option>
                                    <option value="6">SQL Upgrade</option>
								</select>
							</th>
							<th>
								<input class="search" type="text" placeholder="(All)" />
							</th>
                            <th>
                                <!--
								<input class="search supports-operands" type="text" placeholder="(All)" />
                                <select class="operand">
                                    <option value="">=</option>
                                    <option value="1"><</option>
                                    <option value="2">></option>
                                    <option value="3"><=</option>
                                    <option value="4">>=</option>
                                </select> 
                                -->
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
	<!--<input id="pageServerFilter" type="hidden" />
	<input id="serverSelection" type="hidden" /> -->
	<script type="text/javascript">
		$(document).ready(function () {
			//updateSelectedServers();

			setpage();

		    $(document)
		        .on("click",
		            ".expiring-maintenance-window-deletion",
		            function(elm) {
		                var id = $(elm.currentTarget).find("input").val();
		                openConfirmationWindowDeleteExpiring(id);
		            });

		    $(document)
		        .on("click",
		            ".maintenance-window-deletion",
		            function (elm) {
		                var id = $(elm.currentTarget).find("input").val();
		                openConfirmationWindowDelete(id);
		            });
		});

		var windowAttr = 'height = 200, width = 590, location = no, scrollbars = yes, menubar = no, toolbar = no, status = no, resizable = yes';

		function openConfirmationWindowDelete(maintenanceWindowId) {
		    var urlParams = {
		        title: "Remove Maintenance Window",
		        id: maintenanceWindowId,
		        message: "Are you sure you wish to remove this Maintenance Window?"
		    };
            // TODO - Make url factory for local web navigation testing (outside of Relativity)
		    //var deleteUrl = "/ConfirmationPage.aspx?" + $.param(urlParams);
		    var deleteUrl = "/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ConfirmationPage.aspx?" + $.param(urlParams);
		    openConfirmation(deleteUrl);
		}

		function openConfirmationWindowDeleteExpiring(maintenanceWindowId) {
		    var urlParams = {
		        title: "Remove Maintenance Window",
		        id: maintenanceWindowId,
		        message: "Are you sure you wish to remove this Maintenance Window?",
		        warningMessage: "This Window is set to begin within 48 hours and cannot be recreated."
		    };
		    // TODO - Make url factory for local web navigation testing (outside of Relativity)
		    //var deleteUrl = "/ConfirmationPage.aspx?" + $.param(urlParams);
		    var deleteUrl = "/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ConfirmationPage.aspx?" + $.param(urlParams);
		    openConfirmation(deleteUrl);
		}

		//TODO: Move me to pdb common scripts file
		function openConfirmation(url) {
		    var newWindow = window.open(url, '_blank', windowAttr);
		    if (window.focus) {
		        newWindow.focus();
		    }
		}
	</script>
</body>
</html>
