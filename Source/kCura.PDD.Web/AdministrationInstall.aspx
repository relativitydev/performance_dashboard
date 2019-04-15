<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdministrationInstall.aspx.cs" Inherits="kCura.PDD.Web.AdministrationInstall" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard Script Updates</title>
    <script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="Script/AdministrationInstall.js"></script>
    <script type="text/javascript" src="Script/Loading.js"></script>
    <script type="text/javascript" src="Script/bootstrap.min.js"></script>
    <script type="text/javascript" src="Script/bootstrap-switch.min.js"></script>
    <link rel="stylesheet" type="text/css" href="Style/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="Style/bootstrap-switch.min.css" />
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
        <div class="container">
            <div class="row logo-box-a">
                <div class="col-lg-4"></div>
                <div class="col-lg-4 centerContent">
                    <h4 class="logo-m relativity-size-adjust">
                        <img src="Images/rsz_relativitylogo.png" />
                        Performance Dashboard: <span class="pdb-step">Script Updates</span>
                        <img src="Images/infos.png" height="18px" data-toggle="tooltip" data-placement="bottom"
                            title="Some scripts used by Performance Dashboard must be installed with SQL sysadmin privileges. The provided user account must be a sysadmin for all active SQL servers registered with Relativity. Your credentials will not be stored."/>
                    </h4>
                </div>
                <div class="col-lg-4"></div>
            </div>
            <div class="row logo-box-b">
                <div class="col-lg-4"></div>
                <div class="col-lg-4 centerContent">

                    <div runat="server" id="LoginPane" class="well pdb-well relativity-size-adjust hidden-on-success hidden-on-error">
                        <div class="form-horizontal">
                            <fieldset>
                                <legend class="legend-transform-a">
                                    System Administrator Credentials
                                </legend>
                                <div class="form-group">
                                    <label for="useWinAuth" class="col-lg-7 control-label" style="text-align: left;">Use Windows Authentication</label>
                                    <div class="col-lg-5 control-label containsWinAuthToggle" style="text-align: right">
                                        <input type="checkbox" id="useWinAuth" runat="server" class="checkbox" data-size="small" placeholder="UseWinAuth"/>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="inputEmail" class="col-lg-3 control-label">Username</label>
                                    <div class="col-lg-9">
                                        <asp:TextBox ID="databaseWinAuthName" runat="server" class="form-control winAuth" placeholder="Username" ReadOnly="true" />
                                        <asp:TextBox ID="databaseUsername" runat="server" class="form-control formsAuth" placeholder="Username" />
                                    </div>
                                </div>
                                <div class="form-group formsAuth">
                                    <label for="inputPassword" class="col-lg-3 control-label">Password</label>
                                    <div class="col-lg-9">
                                        <asp:TextBox ID="databasePassword" TextMode="Password" runat="server" class="form-control" placeholder="Password" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-10" style="text-align: right; width:100%">
                                        <asp:Button ID="scriptInstallationSubmitButton" OnClick="SubmitInstallationForm" runat="server" class="btn btn-primary" Text="Run" />
                                        <div runat="server" id="scriptInstallationSubmitMockButton" visible="false" class="btn btn-primary disabled">Run</div>
                                        <a href="javascript:history.go(-1)" type="button" class="btn btn-primary" runat="server" ID="scriptsInstallationBackButton">Cancel</a>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    
                    <div style="display: none;" runat="server" id="displaySuccessMessage" visible="false" class="panel panel-success relativity-size-adjust">
                        <div class="panel-heading">
                            <h3 class="panel-title">Performance Dashboard</h3>
                        </div>
                        <div class="panel-body">
                            Performance Dashboard has successfully installed the backup/DBCC monitor scripts.
                            <div style="margin-top:20px; display: none;" class="btn btn-primary btn-xs export-script-install-progress show-export">Details</div>
                        </div>
                    </div>
                    <div style="display: none;" runat="server" id="displayErrorMessage" visible="false" class="panel panel-danger relativity-size-adjust">
                        <div class="panel-heading">
                            <h3 class="panel-title">Performance Dashboard</h3>
                        </div>
                        <div class="panel-body">
                        An error occurred during the installation of backup/DBCC monitor scripts. 
                        Please refresh the page and try again.
                        If the error persists, please contact technical support. You can obtain a detailed log of
                        the installation process by clicking "Details" below.
                        <div style="margin-top:20px; display: none;" class="btn btn-primary btn-xs export-script-install-progress show-export">Details</div>
                        </div>
                    </div>
                    <div style="display: none;" runat="server" id="displayWarningMessage" visible="false" class="panel panel-warning relativity-size-adjust">
                        <div class="panel-heading">
                            <h3 class="panel-title">Performance Dashboard</h3>
                        </div>
                        <div class="panel-body">
                            Authentication using the provided credentials failed, or the user does not have sufficient permissions. Please provide valid sysadmin credentials.
                            <br />
                            <button type="button" class="btn" data-toggle="collapse" data-target="#fullExceptionMessage">More info</button>
                            <div style="overflow: auto" id="fullExceptionMessage" runat="server" class="collapse">

                            </div>
                        </div>
                    </div>
                    
                    <div runat="server" id="InstallationProgressPane" visible="false" style="display: none;">
                        <div class="col-lg-12 well pdb-well relativity-size-adjust">
                            <p style="font-size: 12px;">Results :</p>
                            <div class="progress-window">
                                <ul>
                                    <asp:Repeater ID="scriptInstallationSteps" runat="server">
                                        <ItemTemplate>
                                            <li><%#Eval("InstallationStepValue") %></li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                            <p>
                                <asp:Button ID="requestExportOfInstallationButton" OnClick="RequestExportOfInstallationStep" runat="server" class="btn btn-primary btn-xs export-script-install-progress" Text="Export" />
                            </p>
                            <div class="clear"></div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4">
                </div>
            </div>
        </div>
    </form>
</body>
</html>
