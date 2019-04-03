<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DbccTarget.aspx.cs" Inherits="kCura.PDD.Web.DbccTarget" %>

<%@ Register Assembly="DevExpress.Web.v13.2" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - Manage DBCC Targets</title>
    <script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="Script/Loading.js"></script>
    <script type="text/javascript" src="Script/DbccTargets.js"></script>
    <script type="text/javascript" src="Script/bootstrap.min.js"></script>
    <script type="text/javascript" src="Script/bootstrap-switch.min.js"></script>
    <link rel="stylesheet" type="text/css" href="Style/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="Style/bootstrap-switch.min.css" />
    <link rel="stylesheet" type="text/css" href="Style/Components.css" />
    <link rel="stylesheet" type="text/css" href="Style/AdministrationInstall.css" />
    <link rel="stylesheet" type="text/css" href="Style/Loading.css" />
    <link rel="stylesheet" type="text/css" href="Style/OpenSans.css" />
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
    <form id="DatabaseCredentialForm" runat="server">
        <div class="action-bar">
            <div class="right" style="text-align: center">
                <a href="javascript:history.go($('#postCount').val() * -1)" class="btn">Back</a>
                <a href="#" id="downloadStandardViewButton" runat="server" class="btn skip-loading">Install View Manually</a>
            </div>
            <div class="clear"></div>
        </div>
        <div class="container">
            <div class="row logo-box-a">
                <div class="col-lg-3"></div>
                <div class="col-lg-6 centerContent">
                    <h4 class="logo-m relativity-size-adjust">
                        <img src="Images/rsz_relativitylogo.png" />
                        Performance Dashboard: <span class="pdb-step">DBCC Target Details</span>
                        <img src="Images/infos.png" height="18px" data-toggle="tooltip" data-placement="bottom"
                            title="In order to use view-based DBCC monitoring, at least one SQL server target must be activated. For each monitored server, specify the database containing your DBCC log table. Manual installation of the eddsdbo.QoS_DBCCLog view may be required for custom maintenance solutions."/>
                    </h4>
                </div>
                <div class="col-lg-3"></div>
            </div>
            <div class="row logo-box-b">
                <div class="col-lg-1"></div>
                <div class="col-lg-10 centerContent">
                    <div runat="server" id="SettingsPane" class="well pdb-well relativity-size-adjust">
                        <div class="form-horizontal">
                            <fieldset>
                                <legend class="legend-transform-a">Target Settings
                                </legend>
                                <input type="hidden" id="postCount" runat="server" />
                                <input type="hidden" id="serverName" runat="server" />
                                <input type="checkbox" id="isActive" style="display: none" runat="server" /> 
                                <input type="hidden" id="databaseName" runat="server" />
                                <asp:repeater runat="server" ID="servers">
                                    <HeaderTemplate>
                                        <div class="form-group" style="min-height: 35px">
                                            <div class="col-lg-3">
                                                <label class="control-label" style="font-weight: bold">Server Name</label>
                                            </div>
                                            <div class="col-lg-4">
                                                <label style="margin-top: 5px; font-weight: bold">Target Database</label>
                                                <img src="Images/infos.png" height="11" data-toggle="tooltip" data-placement="bottom"
                            title="The DBCC history view (eddsdbo.QoS_DBCCLog) will be deployed to this database. The default view requires that Ola Hallengren's dbo.CommandLog table is present in the target database and eddsdbo has SELECT permissions on the table. If an existing view with the same name is present, it will not be replaced."/>
                                            </div>
                                            <div class="col-lg-4" style="margin-top: 5px">
                                                <label style="font-weight: bold">Monitoring Enabled</label>
                                                <img src="Images/infos.png" height="11" data-toggle="tooltip" data-placement="bottom"
                            title="All enabled servers will be used as view-based monitoring targets. At least one server must be enabled to use view-based monitoring."/>
                                            </div>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="form-group" style="min-height: 35px; font-weight: normal">
                                            <div class="col-lg-3">
                                                <label style="margin-top: 5px"><%# Eval("Server") %></label>
                                            </div>
                                            <div class="col-lg-4">
                                                <label class="view-mode display-database" style="margin-top: 5px; font-weight: normal"><%# Eval("Database") %></label>
                                                <input type="text" class="form-control current-database edit-mode" style="display: none" placeholder="EDDSQoS" value="<%# Eval("Database") %>"/>
                                            </div>
                                            <div class="col-lg-3 view-mode" style="margin-top: 5px;">
                                                <label class="display-active" style="font-weight: normal"><%# ((bool)Eval("IsActive")) ? "Enabled" : "Disabled" %></label>
                                            </div>
                                            <div class="col-lg-3 edit-mode" style="display: none; margin-top: 5px">
                                                <input type="checkbox" class="checkbox current-active" data-size="small" <%# ((bool)Eval("IsActive")) ? "checked=\"checked\"" : "" %> />
                                            </div>
                                            <div class="col-lg-1" style="margin-top: 5px; width: 7%">
                                                <div class="btn view-mode edit-button">Edit</div>
                                                <div class="btn edit-mode save-button" style="display: none;">Save</div>
                                            </div>
                                            <div class="col-lg-1" style="margin-top: 5px">
                                                <div class="btn edit-mode cancel-button" style="display: none;">Cancel</div>
                                            </div>
                                            <input class="server-id" type="hidden" value="<%# Eval("Id") %>" />
                                            <input class="server-name" type="hidden" value="<%# Eval("Server") %>" />
                                            <input class="original-database" type="hidden" value="<%# Eval("Database") %>" />
                                            <input style="display: none" class="original-active" type="checkbox" <%# ((bool)Eval("IsActive")) ? "checked=\"checked\"" : "" %> />
                                        </div>
                                    </ItemTemplate>
                                </asp:repeater>
                                <input type="hidden" id="targetId" runat="server"/>
                            </fieldset>
                        </div>
                    </div>

                    <div style="display: none;" runat="server" id="displaySuccessMessage" visible="false" class="panel panel-success relativity-size-adjust">
                        <div class="panel-heading">
                            <h3 class="panel-title">Performance Dashboard</h3>
                        </div>
                        <div class="panel-body" id="successMessage" runat="server">
                            Target details have been saved.
                        </div>
                    </div>
                    <div style="display: none;" runat="server" id="displayErrorMessage" visible="false" class="panel panel-danger relativity-size-adjust">
                        <div class="panel-heading">
                            <h3 class="panel-title">Performance Dashboard</h3>
                        </div>
                        <div class="panel-body">
                            Target details could not be saved.
                        </div>
                    </div>
                    <div style="display: none;" runat="server" id="displayWarningMessage" visible="false" class="panel panel-warning relativity-size-adjust">
                        <div class="panel-heading">
                            <h3 class="panel-title">Performance Dashboard</h3>
                        </div>
                        <div id="validationMessage" class="panel-body" runat="server">
                            The connection test for the specified target database failed. The server could be unreachable, the database may not exist, or eddsdbo might not have permission to access it.
                        </div>
                    </div>
                </div>
                <div class="col-lg-1">
                </div>
            </div>
        </div>
    </form>
</body>
</html>
