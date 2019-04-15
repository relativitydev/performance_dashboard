<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ConfirmationPage.aspx.cs" Inherits="kCura.PDD.Web.ConfirmationPage" %>
<!DOCTYPE html>

<html>
<head>
    <link rel="stylesheet" type="text/css" href="Style/ConfirmationPage.css" />
    <script type="text/javascript" src="Script/jquery-3.3.1.min.js"></script>
    <meta name="viewport" content="width=device-width"/>
    <title></title>
</head>
<body>
<%= System.Web.Helpers.AntiForgery.GetHtml() %>
<div style="background-color: white">
    <div id="_main" class="mainClass cssConfirmBackground">
        <div class="titleContainer">
            <span id="TitleText" class="popupTitle" runat="server">TO BE REPLACED WITH REAL TITLE</span>
        </div>
        <div id="CascadeDeleteContainer" class="cssCascadeContainer">
            <span id="MessageText" runat="server">
					TO BE REPLACED WITH REAL MESSAGE BODY
				</span>
            <p/>
            <span id="WarningMessage" runat="server" style="color: red;">
                    TO BE REPLACED WITH REAL WARNING MESSAGE BODY
                </span>
        </div>
        <div id="_bottomActionBarContainer" style="PADDING-TOP: 5px">
            <div id="_messageArea" style="DISPLAY: none; PADDING-BOTTOM: 5px; PADDING-LEFT: 24px; PADDING-RIGHT: 8px; PADDING-TOP: 5px;">
                <span id="_message"></span>
            </div>
            <div id="_bottomActionBarButtonContainer" class="artifactPopupActionBarFull GluedToBottom" style="MARGIN-TOP: 12px">
                <div id="_okCancelButtonContainer" class="cssOkCancelButtonContainer">
                    <a id="_ok_button" class="popupButton" onclick="redirectParent();">OK
                        <input id="WindowId" type="hidden" runat="server" />
                    </a>
                    <a onclick="window.close();" id="_cancel_anchor" class="popupButton">Cancel</a>
                </div>
                <div style="FLOAT: right; MARGIN: -5px; PADDING-BOTTOM: 5px; PADDING-LEFT: 5px; PADDING-RIGHT: 5px; PADDING-TOP: 5px;">
                </div>
            </div>
        </div>
        <div id="MessageBar" style="DISPLAY: none">
            <div id="genericMessageBarFrameContainer">
                <div id="genericMessageImageContainer"></div>
                <div id="genericMessageHTMLContainer"></div>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    function redirectParent() {
        var csrfToken = $("input[name='__RequestVerificationToken']").val();
	    var id = $('#WindowId').val();
        var url = "api/MaintenanceWindowScheduler/DeleteMaintenanceWindow/" + id;

        $.ajax({
            type: "DELETE",
            url: url,
            headers: { "X-CSRF-Header": csrfToken },
            success: function(msg) {
                console.log(msg);
                window.opener.document.location.reload();
                self.close();
            }
        });
    };
</script>
</body>
</html>