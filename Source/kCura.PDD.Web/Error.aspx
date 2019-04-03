<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="kCura.PDD.Web.Error" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard - Error</title>
    <script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="Script/bootstrap.min.js"></script>
    <link rel="stylesheet" type="text/css" href="Style/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="Style/AdministrationInstall.css" />
    <link rel="stylesheet" type="text/css" href="Style/OpenSans.css" />
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <br />
        <div id="displayErrorMessage" class="panel panel-danger relativity-size-adjust" style="width: 20%">
            <div class="panel-heading">
                <h3 class="panel-title">Performance Dashboard</h3>
            </div>
            <div class="panel-body" id="ErrorGeneral" runat="server">
                Sorry, there was an error. Please use your browser's back button to return to the
                previous page or click <a href="javascript:history.go(-1)">here</a> to go back.
            </div>
            <div class="panel-body" id="ReportDataPending" runat="server">
                This page is currently unavailable because quality of service metrics have not been collected. Ensure the QoS Manager agent and at least
                one QoS Worker agent are enabled and try again later.
                <br /><br />
                Please use your browser's back button to return to the
                previous page or click <a href="javascript:history.go(-1)">here</a> to go back.
            </div>
            <div class="panel-body" id="SyncError" runat="server">
                Performance Dashboard's custom pages are out of sync with the deployed version of the application. Please ensure that custom pages
                were properly deployed after installation of the latest version.
                <br /><br />
                Please use your browser's back button to return to the
                previous page or click <a href="javascript:history.go(-1)">here</a> to go back.
            </div>
            <div class="panel-body" id="AccessError" runat="server">
                Access to Performance Dashboard was denied because your session is not attached to the admin workspace.
                <br /><br />
                If you are currently logged into the admin workspace, reload the page or click one of the Performance Dashboard tabs.
            </div>
            <div class="panel-body" id="Error51773" runat="server">
                Performance Dashboard has been disabled due to a fatal error (ERROR_CODE: 51773).
                <br /><br />
                Please contact a kCura support representative for further assistance. The error code above will aid in identifying the source of the problem.
            </div>
        </div>
    </form>
</body>
</html>
