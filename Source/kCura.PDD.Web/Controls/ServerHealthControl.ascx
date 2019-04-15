<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerHealthControl.ascx.cs"
    Inherits="kCura.PDD.Web.Controls.ServerHealthControl" %>
<%@ Register Assembly="DevExpress.XtraCharts.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraCharts" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v13.2" Namespace="DevExpress.Web.ASPxEditors"
    TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.2" Namespace="DevExpress.Web.ASPxGridView"
    TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.2" Namespace="DevExpress.Web.ASPxGridView.Export"
    TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v13.2.Web" Namespace="DevExpress.XtraCharts.Web"
    TagPrefix="dxchartsui" %>
<%@ Register Assembly="DevExpress.Web.v13.2" Namespace="DevExpress.Web.ASPxSplitter"
    TagPrefix="dx" %>
<%@ Register Src="QueryDisplayControl.ascx" TagName="QueryDisplayControl" TagPrefix="uc1" %>

<script src="Script/jquery-3.3.1.min.js" type="text/javascript"></script>
<script src="Script/BrowserDetect.js" type="text/javascript"></script>
<script src="Script/DateFormat.js" type="text/javascript"></script>
<script src="Script/cookie.js" type="text/javascript"></script>
<script src="Script/PreDefinedDateRanges.js" type="text/javascript"></script>
<script type="text/javascript" src="Script/bootstrap.min.js"></script>
<link rel="stylesheet" type="text/css" href="Style/bootstrap.min.css"/>
<link rel="stylesheet" type="text/css" href="Style/Components.css" />
<link rel="stylesheet" type="text/css" href="Style/OpenSans.css" />
<script type="text/javascript">
    $(document).ready(function () {
        SetSize();
        $('[title][title!=""]').tooltip();
    });
	$(window).resize(function () {
	    SetSize();
	});

	function OnSplitterPaneResized(s, e) {
	    var name = e.pane.name;
	    if (name == 'GridContainer')
	        SetGridHeight(e.pane);
	}
	function SetHeightWidth(Width, Height) {
	    var Size = Width.toString() + "," + Height.toString();
	    $("#<%=paneWidthHeight.ClientID %>").val(Size);
    }
    function ZoomChartClick() {
        var HealthSplitter = '<%=HealthSplitter.ClientID %>';
        var splitterChartHeight = $("#" + HealthSplitter + "_0i1").height();
        var splitterGridHeight = $("#" + HealthSplitter + "_0i0").height();
        var width = $("#" + HealthSplitter + "_0i1_CC").width() - 60;
        var height = (splitterGridHeight == 0 ? splitterChartHeight : $("#" + HealthSplitter + "_0").height() - (splitterChartHeight == 0 ? 300 : splitterGridHeight)) - 100;
        if (height < 0) {
            height = $("#" + HealthSplitter + "_0").height() / 2;
        }
        SetHeightWidth(width, height);
        return true;
    }
</script>
<script type="text/javascript">
    var HealthGridView = '<%= DynamicHealthGridView.ClientID %>';   
</script>
<style media="screen">
    #<%= ServerAreaComboBox.ClientID %> input
    {
        height: 13px;        
        margin: 0px
    }    
    #<%= ServerAreaComboBox.ClientID %> img
    {
        height: 13px;
        margin-top: -1px
    }
    </style>
<link rel="Stylesheet" type="text/css" href="Style/CommonStyle.css?<%= Application["ApplicationId"].ToString() %>" />
<script src="Script/date.js?<%= Application["ApplicationId"].ToString() %>" type="text/javascript"></script>
<script src="Script/ServerHealth.js?<%= Application["ApplicationId"].ToString() %>"
    type="text/javascript"></script>
<div class="action-bar" style="position: relative">
        <div class="right" style="text-align: center; margin-top: 2px">
            <a href="javascript:history.go(-1)" type="button" class="btn">Back</a>
            <a href="#" target="_parent" id="QoSNavButton" class="btn" runat="server">QoS Report</a>
        </div>
        <div class="clear"></div>
    </div>
<div id="ErrorDiv" class="errorMessageLightBlueBackGround">
</div>
<div class="mainClass" id="_main" style="margin: 2px; min-width: 700px;">
    <input type="hidden" id="paneWidthHeight" runat="server" />
    <input type="hidden" id="hdnTimeZoneMinutes" runat="server" />
    <div class="SearchViewTemplateHeader pivotcontainer" id="SearchViewDiv">
        <div style="border-bottom: black 1px solid" class="pivotActionBar">
            <div>
                <span style="padding-left: 3px; display: inline; float: right;"></span><span style="border-left: 1px solid; padding-left: 3px; display: inline; float: right">
                    <table cellspacing="0" cellpadding="1">
                        <tbody>
                            <tr>
                                <td style="text-align: right;">
                                    <label id="StartDateLabel" class="labelstyle">
                                        Start Date :&nbsp;</label>
                                </td>
                                <td id="tdStartDate">
                                    <dx:ASPxDateEdit ID="StartDateEdit" AllowUserInput="true" Font-Size="8" Font-Names="Open Sans"
                                        ClientInstanceName="StartDateEditClient" Width="100px" EditFormat="Date" UseMaskBehavior="false"
                                        ClientSideEvents-Init="function(s, e) { s.SetMaxDate(new Date()); }" runat="server">
                                        <ClientSideEvents ValueChanged="UpdateAllowedDates" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="ShowDataButton" Text="Go" Width="50px" Height="18px" HorizontalAlign="Center"
                                        CssClass="cmdButtons" Cursor="pointer" EnableDefaultAppearance="false" runat="server"
                                        ClientSideEvents-Click="function(s, e) { e.processOnServer = CheckStartEndDate(); if (e.processOnServer) DisableGo(); }"
                                        OnClick="ShowDataButton_Click" />
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="DateRangesComboBox" Width="125px" runat="server" Font-Names="Open Sans"
                                        Font-Size="8" ClientInstanceName="DateRangesComboBoxClient">
                                        <Items>
                                            <dx:ListEditItem Value="0" Text="Date Ranges..." Selected="true" />
                                            <dx:ListEditItem Value="1" Text="Today" />
                                            <dx:ListEditItem Value="2" Text="Yesterday" />
                                            <dx:ListEditItem Value="3" Text="Past 7 Days" />
                                            <dx:ListEditItem Value="4" Text="Past Month" />
                                            <dx:ListEditItem Value="5" Text="Past 3 Months" />
                                        </Items>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { PreDefinedDateRangeSelected(); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <label id="EndDateLabel" class="labelstyle">
                                        End Date :&nbsp;</label>
                                </td>
                                <td id="tdEndDate">
                                    <dx:ASPxDateEdit ID="EndDateEdit" AllowUserInput="true" Font-Size="8" Font-Names="Open Sans"
                                        ClientInstanceName="EndDateEditClient" Width="100px" EditFormat="Date" UseMaskBehavior="false"
                                        ClientSideEvents-Init="function(s, e) { s.SetMaxDate(new Date()); }" runat="server">
                                        <ClientSideEvents ValueChanged="UpdateAllowedDates" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="ClearDataButton" Width="50px" Height="18px" HorizontalAlign="Center"
                                        Text="Clear" CssClass="cmdButtons" Cursor="pointer" EnableDefaultAppearance="false"
                                        runat="server" OnClick="ClearDataButton_Click" />
                                </td>
                                <td></td>
                            </tr>
                        </tbody>
                    </table>
                </span>
                <div id="BestInServiceScoreContainer" runat="server" class="scoreContainer">
                        <a href="#" target="_parent" ID="BestInServiceScore" runat="server" title="Quarterly" data-placement="bottom"></a>
                        <a href="#" target="_parent" ID="WeeklyScore" runat="server" title="Weekly" data-placement="bottom"></a>
                </div>
                <div style="float: left; vertical-align: middle; padding-top: 5px; width: 48%">
                    <span class="regularTitle" style="vertical-align: middle;">Performance Dashboard:
                        <asp:Literal ID="ltrTitle" runat="server"></asp:Literal>
                    </span>
                    <asp:PlaceHolder ID="ServerAreaComboBoxPlaceHolder" runat="server"><span style="display: inline-table; *display: inline-block; vertical-align: middle;">
                        <div style="vertical-align: top;">
                            <dx:ASPxComboBox ID="ServerAreaComboBox" Width="100" runat="server" Font-Names="Open Sans"
                                Font-Size="8" AutoPostBack="true" OnSelectedIndexChanged="ServerAreaComboBox_SelectedIndexChanged">
                                <Items>
                                    <dx:ListEditItem Value="2" Text="Memory (RAM)" Selected="true" />
                                    <dx:ListEditItem Value="4" Text="Processor" />
                                    <dx:ListEditItem Value="3" Text="Storage" />
                                    <dx:ListEditItem Value="5" Text="SQL Server" />
                                </Items>
                            </dx:ASPxComboBox>
                        </div>
                    </span></asp:PlaceHolder>
                    <div style="padding-left: 6px; padding-top: 3px;">
                        from
                        <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                        through
                        <asp:Label ID="lblEndDate" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
            <div style="text-align: left; padding-bottom: 3px; padding-left: 0px; padding-right: 0px; display: block; padding-top: 3px">
                <span style="display: block">
                    <asp:ImageButton ImageUrl="~/Images/Grid_on.png" ID="GridImageButton" runat="server" data-placement="bottom"
                        Style="border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; margin-left: 5px; border-left-width: 0px; margin-right: 5px; cursor: pointer"
                        ToolTip="Toggle Grid On/Off" OnClick="GridImageButton_Click" OnClientClick="return ZoomChartClick();" />
                    <asp:ImageButton ImageUrl="~/Images/Chart_off.png" ID="ChartImageButton" runat="server" data-placement="bottom"
                        OnClick="ChartImageButton_Click" Style="border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; margin-left: 5px; border-left-width: 0px; margin-right: 5px; cursor: pointer;"
                        ToolTip="Toggle Chart On/Off" OnClientClick="return ZoomChartClick();"
                        Height="40px" />
                </span>
            </div>
        </div>
    </div>
    <div>
        <dx:ASPxSplitter ID="HealthSplitter" SeparatorSize="2" ShowSeparatorImage="true"
            Width="100%" runat="server" AllowResize="true" ClientInstanceName="splitter">

            <ClientSideEvents PaneResized="OnSplitterPaneResized" />
            <Panes>
                <dx:SplitterPane Size="100%">
                    <Panes>
                        <dx:SplitterPane Name="GridContainer" Size="39%" AllowResize="True" PaneStyle-BackColor="#D6E3F7"
                            MinSize="100px">
                            <PaneStyle BackColor="#D6E3F7">
                            </PaneStyle>
                            <ContentCollection>
                                <dx:SplitterContentControl ID="SplitterContentControl1" runat="server">
                                    <table class="itemListTable" border="0" cellspacing="0" cellpadding="0" width="100%">
                                        <tbody>
                                            <tr>
                                                <td style="background-color: #fff">
                                                    <div>
                                                        <div class="actionCellContainer leftAligned">
                                                            <div style="padding-top: 2px; padding-left: 6px; padding-bottom: 2px">
                                                                <table cellpadding="0" cellspacing="0" border="0">
                                                                    <tr>
                                                                        <td style="padding-right: 4px">Export to
                                                                        </td>
                                                                        <td style="padding-right: 4px">
                                                                            <dx:ASPxButton ID="btnCsvExport" EnableDefaultAppearance="true" Width="35" Height="18"
                                                                                Paddings-Padding="0" FocusRectPaddings-Padding="0" Font-Size="8" Font-Names="Open Sans"
                                                                                CssClass="btnExport" runat="server" Text="CSV" UseSubmitBehavior="False"
                                                                                OnClick="btnCsvExport_Click">
                                                                                <Paddings Padding="0px"></Paddings>

                                                                                <FocusRectPaddings Padding="0px"></FocusRectPaddings>
                                                                            </dx:ASPxButton>
                                                                        </td>
                                                                        <td style="padding-right: 4px">
                                                                            <dx:ASPxButton ID="btnXlsExport" EnableDefaultAppearance="true" Width="35" Height="18"
                                                                                Paddings-Padding="0" FocusRectPaddings-Padding="0" Font-Size="8" Font-Names="Open Sans"
                                                                                CssClass="btnExport" runat="server" Text="XLS" UseSubmitBehavior="False"
                                                                                OnClick="btnXlsExport_Click">
                                                                                <Paddings Padding="0px"></Paddings>

                                                                                <FocusRectPaddings Padding="0px"></FocusRectPaddings>
                                                                            </dx:ASPxButton>
                                                                        </td>
                                                                        <td style="padding-right: 4px">
                                                                            <dx:ASPxButton ID="btnXlsxExport" EnableDefaultAppearance="true" Width="40" Height="18"
                                                                                Paddings-Padding="0" FocusRectPaddings-Padding="0" Font-Size="8" Font-Names="Open Sans"
                                                                                CssClass="btnExport" runat="server" Text="XLSX" UseSubmitBehavior="False"
                                                                                OnClick="btnXlsxExport_Click">
                                                                                <Paddings Padding="0px"></Paddings>

                                                                                <FocusRectPaddings Padding="0px"></FocusRectPaddings>
                                                                            </dx:ASPxButton>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                            <%--<span style="height: 100%; padding-top: 3px" class="itemListActionCell"><span id="numOfSelectedItems">
                                        0</span>&nbsp;&nbsp;<span id="ctl00_ctl00_itemList_lblSelectedItems">Selected Item(s)</span></span>--%>
                                                        </div>
                                                        <div class="actionCellContainer rightAligned" style="margin-right: 25px">
                                                            <span class="itemListActionCell">
                                                                <dx:ASPxHyperLink ID="ShowFiltersLink" CssClass="itemListActionImage" ImageUrl="~/Images/itemList_filter.png" EnableDefaultAppearance="false" ImageWidth="16px"
                                                                    ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" ToolTip="Show Filters" />
                                                                <dx:ASPxHyperLink ID="ApplyFilterLink" CssClass="itemListActionImage" ImageUrl="~/Images/apply_filters.png" EnableDefaultAppearance="false" Enabled="false" ImageWidth="16px"
                                                                    ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" ToolTip="Apply Filters" />
                                                                <dx:ASPxHyperLink ID="ClearAllLink" CssClass="itemListActionImage" ImageUrl="~/Images/clear_filters.png" Enabled="false" EnableDefaultAppearance="false" ImageWidth="16px"
                                                                    ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" ToolTip="Clear All" />
                                                            </span>
                                                            <span class="itemListActionCell">&nbsp;|&nbsp;Page&nbsp;</span>
                                                            <span class="itemListActionCell">
                                                                <asp:HiddenField ID="CurrentPageHidden" runat="server" Value="1" />
                                                            </span><span class="itemListActionCell">
                                                                <dx:ASPxLabel ID="CurrentPageLabel" runat="server">
                                                                </dx:ASPxLabel>
                                                                (<span>of</span>&nbsp;
                                                                <dx:ASPxLabel ID="TotalPageLabel" runat="server">
                                                                </dx:ASPxLabel>
                                                                )&nbsp;</span> <span class="itemListActionCell" style="padding-top: 2px;">
                                                                    <dx:ASPxHyperLink ID="FirstDataLink" ImageUrl="~/Images/first.png" ImageWidth="16px"
                                                                        ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" />
                                                                    <dx:ASPxHyperLink ID="PreviousDataLink" ImageUrl="~/Images/previous.png" ImageWidth="16px"
                                                                        ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" />
                                                                    <dx:ASPxHyperLink ID="NextDataLink" ImageUrl="~/Images/next.png" ImageWidth="16px"
                                                                        ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" />
                                                                    <dx:ASPxHyperLink ID="LastDataLink" ImageUrl="~/Images/last.png" ImageWidth="16px"
                                                                        ImageHeight="16px" runat="server" NavigateUrl="javascript:void(0)" />
                                                                </span><span class="itemListActionCell">&nbsp;|&nbsp;viewing&nbsp;</span> <span class="itemListActionCell">
                                                                    <dx:ASPxLabel ID="TotalRecordsLabel" Font-Bold="true" EnableDefaultAppearance="false"
                                                                        runat="server">
                                                                    </dx:ASPxLabel>
                                                                </span><span class="itemListActionCell">&nbsp;items in set of&nbsp; </span><span
                                                                    class="itemListActionCell">
                                                                    <dx:ASPxComboBox ID="PageDropDownList" runat="server" AutoPostBack="true" Font-Size="8" Font-Names="Open Sans" Width="50"
                                                                        OnSelectedIndexChanged="PageDropDownList_SelectedIndexChanged">
                                                                        <Items>
                                                                            <dx:ListEditItem Text="10" Value="10" />
                                                                            <dx:ListEditItem Text="15" Value="15" />
                                                                            <dx:ListEditItem Text="20" Value="20" />
                                                                            <dx:ListEditItem Text="25" Value="25" />
                                                                            <dx:ListEditItem Text="50" Value="50" />
                                                                            <dx:ListEditItem Text="100" Value="100" />
                                                                            <dx:ListEditItem Text="200" Value="200" />
                                                                            <dx:ListEditItem Text="500" Value="500" />
                                                                            <dx:ListEditItem Text="1000" Value="1000" />
                                                                        </Items>
                                                                    </dx:ASPxComboBox>
                                                                </span><span class="itemListActionCell">&nbsp;&nbsp;per page </span>
                                                        </div>
                                                        <div style="clear: both">
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="GridParentDiv">
                                                        <!-- Application Health Grid View -->
                                                        <dx:ASPxGridView ID="ApplicationHealthGridView" Width="100%" KeyFieldName="Id" runat="server"
                                                            ClientInstanceName="grid" OnHtmlDataCellPrepared="ApplicationHealthGridView_HtmlDataCellPrepared">

                                                            <Settings ShowVerticalScrollBar="True"></Settings>

                                                            <Styles>
                                                                <%--<Header CssClass="itemListHeadFilter" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                    Font-Underline="True" Cursor="pointer" />--%>
                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE" />
                                                                <Row CssClass="itemListGridCellStale" />
                                                                <SelectedRow BackColor="#CCCCCC" />
                                                                <Row CssClass="itemListGridCellStale"></Row>

                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE"></AlternatingRow>

                                                                <SelectedRow BackColor="#CCCCCC"></SelectedRow>

                                                                <Cell Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </Cell>
                                                                <CommandColumn Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </CommandColumn>
                                                            </Styles>
                                                            <ClientSideEvents SelectionChanged="GridSelectionChanged" Init="function() { SelectAllCheckBox(); }"></ClientSideEvents>
                                                            <Columns>
                                                                <dx:GridViewCommandColumn Width="50" ShowSelectCheckbox="true" VisibleIndex="0">
                                                                    <HeaderTemplate>
                                                                        <dx:ASPxCheckBox ID="SelectAllCheckBox" ClientInstanceName="SelectAllCheckBoxClient"
                                                                            runat="server" ToolTip="Select/Unselect all rows on the page" ClientSideEvents-CheckedChanged="function(sender, event) { SelectUnSelectCheckBox(sender); }" />
                                                                    </HeaderTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" CssClass="itemListHead" Border-BorderColor="#FFFFFF"
                                                                        ForeColor="whitesmoke" Font-Underline="false">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewCommandColumn>
                                                                <dx:GridViewDataColumn FieldName="WorkspaceName" SortOrder="Ascending" VisibleIndex="1">
                                                                    <HeaderCaptionTemplate>
                                                                        Workspace Name<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="ArtifactID" VisibleIndex="2">
                                                                    <HeaderCaptionTemplate>
                                                                        Artifact ID<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                    <CellStyle HorizontalAlign="Left"></CellStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="SQLInstanceName" VisibleIndex="3">
                                                                    <HeaderCaptionTemplate>
                                                                        SQL Instance Name<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="LRQ" VisibleIndex="4">
                                                                    <HeaderCaptionTemplate>
                                                                        Long Running Queries<dx:ASPxImage runat="server" data-placement="bottom" ToolTip="A combination of simple document queries that took longer than 2 seconds and complex document queries that took longer than 8 seconds to complete in the selected time frame. This calculation is based on the selected time frame and your local time." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="Errors" VisibleIndex="5">
                                                                    <HeaderCaptionTemplate>
                                                                        Critical Errors<dx:ASPxImage runat="server" data-placement="bottom" ToolTip="The total number of errors that occurred in the selected time frame.  An error can be one of the following: Read Failed, Delete Failed, Create Failed, Update Failed, Object reference not set to an instance of an object,  SQL Statement Failed, Unable to connect to the remote server. This calculation is based on the selected time frame and your local time." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="Users" Caption="Peak Users" VisibleIndex="7">
                                                                    <HeaderCaptionTemplate>
                                                                        Peak Users<dx:ASPxImage runat="server" data-placement="bottom" ToolTip="The average peak user count in the selected time frame. The peak user count is calculated per day. This calculation is based on the selected time frame and your local time." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                            </Columns>
                                                            <ClientSideEvents Init="function() { SelectAllCheckBox(); }" SelectionChanged="GridSelectionChanged" />
                                                            <SettingsPager Visible="false">
                                                                <FirstPageButton Text="F">
                                                                </FirstPageButton>
                                                                <PrevPageButton Text="P">
                                                                </PrevPageButton>
                                                                <NextPageButton Text="N">
                                                                </NextPageButton>
                                                                <LastPageButton Text="L">
                                                                </LastPageButton>
                                                            </SettingsPager>
                                                            <SettingsBehavior ColumnResizeMode="Control"></SettingsBehavior>
                                                            <Settings ShowVerticalScrollBar="true" />
                                                        </dx:ASPxGridView>
                                                        <!-- Ram Grid -->
                                                        <dx:ASPxGridView ID="RamHealthGridView" Width="100%" KeyFieldName="Id" runat="server"
                                                            ClientInstanceName="grid" AutoGenerateColumns="false">

                                                            <Settings ShowVerticalScrollBar="True"></Settings>

                                                            <Styles>
                                                                <%--<Header CssClass="itemListHeadFilter" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                    Font-Underline="True" Cursor="pointer" />--%>
                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE" />
                                                                <Row CssClass="itemListGridCellStale" />
                                                                <SelectedRow BackColor="#CCCCCC" />
                                                                <Row CssClass="itemListGridCellStale"></Row>

                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE"></AlternatingRow>

                                                                <SelectedRow BackColor="#CCCCCC"></SelectedRow>

                                                                <Cell Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </Cell>
                                                                <CommandColumn Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </CommandColumn>
                                                            </Styles>
                                                            <ClientSideEvents SelectionChanged="GridSelectionChanged" Init="function() { SelectAllCheckBox(); }"></ClientSideEvents>
                                                            <Columns>
                                                                <dx:GridViewCommandColumn Width="50" ShowSelectCheckbox="true" VisibleIndex="0">
                                                                    <HeaderTemplate>
                                                                        <dx:ASPxCheckBox ID="SelectAllCheckBox" ClientInstanceName="SelectAllCheckBoxClient"
                                                                            runat="server" ToolTip="Select/Unselect all rows on the page" ClientSideEvents-CheckedChanged="function(sender, event) { SelectUnSelectCheckBox(sender); }" />
                                                                    </HeaderTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" CssClass="itemListHead" Border-BorderColor="#FFFFFF"
                                                                        ForeColor="whitesmoke" Font-Underline="false">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewCommandColumn>
                                                                <dx:GridViewDataColumn FieldName="Name" VisibleIndex="1">
                                                                    <HeaderCaptionTemplate>
                                                                        Name<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="Type" SortOrder="Ascending" VisibleIndex="2">
                                                                    <HeaderCaptionTemplate>
                                                                        Type<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="PagesPerSec" VisibleIndex="3">
                                                                    <HeaderCaptionTemplate>
                                                                        Pages/sec<dx:ASPxImage runat="server" data-placement="bottom" ToolTip="The number of pages that are either read from disk to resolve hard page faults or written to disk to free up memory.  A hard page fault occurs when a process references a page that is no longer located in physical memory." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="PageFaultsPerSec" VisibleIndex="4">
                                                                    <HeaderCaptionTemplate>
                                                                        Page Faults/sec<dx:ASPxImage runat="server" data-placement="bottom" ToolTip="This is a combination of hard page faults and soft page faults. A hard page fault occurs when a process references a page that is no longer located in physical memory.   A soft page fault occurs with the page actually is in memory, but is not marked in the memory management unit as being active. Unlike a hard page fault, a soft page fault does not require a disk read." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <%--<dx:GridViewDataColumn FieldName="Percentage" VisibleIndex="4">
                                                                    <HeaderCaptionTemplate>
                                                                        RAM Usage<dx:ASPxImage runat="server" ToolTip="This shows the average percentage of physical memory in use over the selected time range." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>--%>
                                                            </Columns>
                                                            <ClientSideEvents Init="function() { SelectAllCheckBox(); }" SelectionChanged="GridSelectionChanged" />
                                                            <SettingsPager Visible="false">
                                                                <FirstPageButton Text="F">
                                                                </FirstPageButton>
                                                                <PrevPageButton Text="P">
                                                                </PrevPageButton>
                                                                <NextPageButton Text="N">
                                                                </NextPageButton>
                                                                <LastPageButton Text="L">
                                                                </LastPageButton>
                                                            </SettingsPager>
                                                            <SettingsBehavior ColumnResizeMode="Control"></SettingsBehavior>
                                                            <Settings ShowVerticalScrollBar="true" />
                                                        </dx:ASPxGridView>
                                                        <!-- Processor Grid -->
                                                        <dx:ASPxGridView ID="ProcessorHealthGridView" Width="100%" KeyFieldName="Id" runat="server"
                                                            ClientInstanceName="grid" AutoGenerateColumns="false">

                                                            <Settings ShowVerticalScrollBar="True"></Settings>

                                                            <Styles>
                                                                <%--<Header CssClass="itemListHeadFilter" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                    Font-Underline="True" Cursor="pointer" />--%>
                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE" />
                                                                <Row CssClass="itemListGridCellStale" />
                                                                <SelectedRow BackColor="#CCCCCC" />
                                                                <Row CssClass="itemListGridCellStale"></Row>

                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE"></AlternatingRow>

                                                                <SelectedRow BackColor="#CCCCCC"></SelectedRow>

                                                                <Cell Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </Cell>
                                                                <CommandColumn Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </CommandColumn>
                                                            </Styles>
                                                            <ClientSideEvents SelectionChanged="GridSelectionChanged" Init="function() { SelectAllCheckBox(); }"></ClientSideEvents>
                                                            <Columns>
                                                                <dx:GridViewCommandColumn Width="50" ShowSelectCheckbox="true" VisibleIndex="0">
                                                                    <HeaderTemplate>
                                                                        <dx:ASPxCheckBox ID="SelectAllCheckBox" ClientInstanceName="SelectAllCheckBoxClient"
                                                                            runat="server" ToolTip="Select/Unselect all rows on the page" ClientSideEvents-CheckedChanged="function(sender, event) { SelectUnSelectCheckBox(sender); }" />
                                                                    </HeaderTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" CssClass="itemListHead" Border-BorderColor="#FFFFFF"
                                                                        ForeColor="whitesmoke" Font-Underline="false">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewCommandColumn>
                                                                <dx:GridViewDataColumn FieldName="Name" VisibleIndex="1">
                                                                    <HeaderCaptionTemplate>
                                                                        Name<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="Type" SortOrder="Ascending" VisibleIndex="2">
                                                                    <HeaderCaptionTemplate>
                                                                        Type<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="ProcessorTime" VisibleIndex="3">
                                                                    <HeaderCaptionTemplate>
                                                                        Processor Time<dx:ASPxImage runat="server" ToolTip="This shows the percentage of time the processor was busy over the selected time range." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                            </Columns>
                                                            <ClientSideEvents Init="function() { SelectAllCheckBox(); }" SelectionChanged="GridSelectionChanged" />
                                                            <SettingsPager Visible="false">
                                                                <FirstPageButton Text="F">
                                                                </FirstPageButton>
                                                                <PrevPageButton Text="P">
                                                                </PrevPageButton>
                                                                <NextPageButton Text="N">
                                                                </NextPageButton>
                                                                <LastPageButton Text="L">
                                                                </LastPageButton>
                                                            </SettingsPager>
                                                            <SettingsBehavior ColumnResizeMode="Control"></SettingsBehavior>
                                                            <Settings ShowVerticalScrollBar="true" />
                                                        </dx:ASPxGridView>
                                                        <!-- Hard Disk Health Grid -->
                                                        <dx:ASPxGridView ID="HardDiskHealthGridView" Width="100%" KeyFieldName="Id" runat="server"
                                                            ClientInstanceName="grid" AutoGenerateColumns="false">

                                                            <Settings ShowVerticalScrollBar="True"></Settings>

                                                            <Styles>
                                                                <%--<Header CssClass="itemListHeadFilter" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                    Font-Underline="True" Cursor="pointer" />--%>
                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE" />
                                                                <Row CssClass="itemListGridCellStale" />
                                                                <SelectedRow BackColor="#CCCCCC" />
                                                                <Row CssClass="itemListGridCellStale"></Row>

                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE"></AlternatingRow>

                                                                <SelectedRow BackColor="#CCCCCC"></SelectedRow>

                                                                <Cell Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </Cell>
                                                                <CommandColumn Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </CommandColumn>
                                                            </Styles>
                                                            <ClientSideEvents SelectionChanged="GridSelectionChanged" Init="function() { SelectAllCheckBox(); }"></ClientSideEvents>
                                                            <Columns>
                                                                <dx:GridViewCommandColumn Width="50" ShowSelectCheckbox="true" VisibleIndex="0">
                                                                    <HeaderTemplate>
                                                                        <dx:ASPxCheckBox ID="SelectAllCheckBox" ClientInstanceName="SelectAllCheckBoxClient"
                                                                            runat="server" ToolTip="Select/Unselect all rows on the page" ClientSideEvents-CheckedChanged="function(sender, event) { SelectUnSelectCheckBox(sender); }" />
                                                                    </HeaderTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" CssClass="itemListHead" Border-BorderColor="#FFFFFF"
                                                                        ForeColor="whitesmoke" Font-Underline="false">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewCommandColumn>
                                                                <dx:GridViewDataColumn FieldName="Name" VisibleIndex="1">
                                                                    <HeaderCaptionTemplate>
                                                                        Name<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="Type" SortOrder="Ascending" VisibleIndex="2">
                                                                    <HeaderCaptionTemplate>
                                                                        Type<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="DiskNumber" VisibleIndex="3">
                                                                    <HeaderCaptionTemplate>
                                                                        Disk Index<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="DriveLetter" VisibleIndex="4">
                                                                    <HeaderCaptionTemplate>
                                                                        Drive Letter<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="DiskRead" VisibleIndex="5">
                                                                    <HeaderCaptionTemplate>
                                                                        Disk Read Latency<dx:ASPxImage runat="server" ToolTip="The average time, in seconds, for a read operation from disk to complete." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="DiskWrite" VisibleIndex="6">
                                                                    <HeaderCaptionTemplate>
                                                                        Disk Write Latency<dx:ASPxImage runat="server" ToolTip="The average time, in seconds, for a write operation to disk to complete." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                            </Columns>
                                                            <ClientSideEvents Init="function() { SelectAllCheckBox(); }" SelectionChanged="GridSelectionChanged" />
                                                            <SettingsPager Visible="false">
                                                                <FirstPageButton Text="F">
                                                                </FirstPageButton>
                                                                <PrevPageButton Text="P">
                                                                </PrevPageButton>
                                                                <NextPageButton Text="N">
                                                                </NextPageButton>
                                                                <LastPageButton Text="L">
                                                                </LastPageButton>
                                                            </SettingsPager>
                                                            <SettingsBehavior ColumnResizeMode="Control"></SettingsBehavior>
                                                            <Settings ShowVerticalScrollBar="true" />
                                                        </dx:ASPxGridView>
                                                        <!--Sql Server Health Grid -->
                                                        <dx:ASPxGridView ID="SqlServerHealthGridView" Width="100%" KeyFieldName="Id" runat="server"
                                                            ClientInstanceName="grid" AutoGenerateColumns="false">

                                                            <Settings ShowVerticalScrollBar="True"></Settings>

                                                            <Styles>
                                                                <%--<Header CssClass="itemListHeadFilter" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                    Font-Underline="True" Cursor="pointer" />--%>
                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE" />
                                                                <Row CssClass="itemListGridCellStale" />
                                                                <SelectedRow BackColor="#CCCCCC" />
                                                                <Row CssClass="itemListGridCellStale"></Row>

                                                                <AlternatingRow Enabled="True" BackColor="#EEEEEE"></AlternatingRow>

                                                                <SelectedRow BackColor="#CCCCCC"></SelectedRow>

                                                                <Cell Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </Cell>
                                                                <CommandColumn Border-BorderColor="#D6E3F7">
                                                                    <Border BorderColor="#D6E3F7"></Border>
                                                                </CommandColumn>
                                                            </Styles>
                                                            <ClientSideEvents SelectionChanged="GridSelectionChanged" Init="function() { SelectAllCheckBox(); }"></ClientSideEvents>
                                                            <Columns>
                                                                <dx:GridViewCommandColumn Width="50" ShowSelectCheckbox="true" VisibleIndex="0">
                                                                    <HeaderTemplate>
                                                                        <dx:ASPxCheckBox ID="SelectAllCheckBox" ClientInstanceName="SelectAllCheckBoxClient"
                                                                            runat="server" ToolTip="Select/Unselect all rows on the page" ClientSideEvents-CheckedChanged="function(sender, event) { SelectUnSelectCheckBox(sender); }" />
                                                                    </HeaderTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" CssClass="itemListHead" Border-BorderColor="#FFFFFF"
                                                                        ForeColor="whitesmoke" Font-Underline="false">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewCommandColumn>
                                                                <dx:GridViewDataColumn FieldName="Name" VisibleIndex="1">
                                                                    <HeaderCaptionTemplate>
                                                                        Name<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="Type" SortOrder="Ascending" VisibleIndex="2">
                                                                    <HeaderCaptionTemplate>
                                                                        Type<dx:ASPxImage runat="server" ToolTip="" ImageUrl="~/Images/blank.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                                <dx:GridViewDataColumn FieldName="PageLifeExpectancy" VisibleIndex="3">
                                                                    <HeaderCaptionTemplate>
                                                                        Page Life Expectancy<dx:ASPxImage runat="server" ToolTip="Number of seconds a page will stay in the buffer pool without references." ImageUrl="~/Images/help_icon.png" />
                                                                    </HeaderCaptionTemplate>
                                                                    <HeaderStyle CssClass="itemListHead" Border-BorderColor="#FFFFFF" ForeColor="whitesmoke"
                                                                        Font-Underline="True" Cursor="pointer">
                                                                        <Border BorderColor="White"></Border>
                                                                    </HeaderStyle>
                                                                </dx:GridViewDataColumn>
                                                            </Columns>
                                                            <ClientSideEvents Init="function() { SelectAllCheckBox(); }" SelectionChanged="GridSelectionChanged" />
                                                            <SettingsPager Visible="false">
                                                                <FirstPageButton Text="F">
                                                                </FirstPageButton>
                                                                <PrevPageButton Text="P">
                                                                </PrevPageButton>
                                                                <NextPageButton Text="N">
                                                                </NextPageButton>
                                                                <LastPageButton Text="L">
                                                                </LastPageButton>
                                                            </SettingsPager>
                                                            <SettingsBehavior ColumnResizeMode="Control"></SettingsBehavior>
                                                            <Settings ShowVerticalScrollBar="true" />
                                                        </dx:ASPxGridView>
                                                        <dx:ASPxGridViewExporter ID="ExportGrid" GridViewID="grid" runat="server">
                                                        </dx:ASPxGridViewExporter>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </dx:SplitterContentControl>
                            </ContentCollection>
                        </dx:SplitterPane>
                        <dx:SplitterPane MinSize="100" AllowResize="True" Size="60%" ScrollBars="Auto" Collapsed="true"
                            PaneStyle-BackColor="#D6E3F7" Name="ChartContainer">
                            <PaneStyle BackColor="#D6E3F7">
                            </PaneStyle>
                            <ContentCollection>
                                <dx:SplitterContentControl ID="SplitterContentControl2" runat="server">
                                    <div style="padding: 5px">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ImageUrl="~/Images/sizeToFit.png" ID="FitToScreenImageButton" runat="server"
                                                        Style="border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; margin-left: 5px; border-left-width: 0px; margin-right: 5px"
                                                        ToolTip="Fit to Screen"
                                                        OnClick="FitToScreenImageButton_Click" OnClientClick="return ZoomChartClick();" /></td>
                                                <td>
                                                    <dx:ASPxButton runat="server" ID="QueryPerformanceImageButton" EnableTheming="False" Cursor="pointer" EnableDefaultAppearance="False"
                                                        ClientSideEvents-Click="function(s, e) { e.processOnServer = CheckForOneServerInstance(); }"
                                                        OnClick="QueryPerformanceImageButton_Click">
                                                        <Image Url="~/Images/gear.png" />
                                                    </dx:ASPxButton>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ImageUrl="~/Images/show_all.png" ID="ShowAllButton" runat="server"
                                                        Style="border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; margin-left: 5px; border-left-width: 0px; margin-right: 5px"
                                                        ToolTip="Expand Grid"
                                                        Visible="False" OnClientClick="toggleGridExpand(); return false;" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblErrorMessage" Text="Please select a single SQL Server Instance" Style="display: none; color: red;" ClientIDMode="Static"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>

                                        <%--<dx:ASPxImage ID="FitToScreenImage" ImageUrl="Images/sizeToFit.png" runat="server"
                                                Cursor="pointer" />--%>
                                    </div>
                                    <asp:Panel runat="server" ID="ChartPanel">
                                        <div style="text-align: center;">
                                            <table id="Table1" width="100%" style="border: solid 0px #000" runat="server">
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <label style="margin: 0px 10px">
                                                                        Chart Type
                                                                    </label>
                                                                </td>
                                                                <td>
                                                                    <dx:ASPxComboBox ID="ChartTypeComboBox" runat="server" Font-Size="8" Font-Names="Open Sans">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ chart.PerformCallback(&quot;ChartTypeChanged&quot;); }" />
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ chart.PerformCallback(&quot;ChartTypeChanged&quot;); }"></ClientSideEvents>
                                                                        <Items>
                                                                            <dx:ListEditItem Text="Line Graph" Selected="true" Value="1" />
                                                                            <dx:ListEditItem Text="Bar Graph" Value="2" />
                                                                        </Items>
                                                                    </dx:ASPxComboBox>
                                                                </td>
                                                                <td>
                                                                    <label style="margin: 0px 10px">
                                                                        Show Columns</label>
                                                                </td>
                                                                <td>
                                                                    <dx:ASPxComboBox ID="ShowColumnsComboBox" runat="server" Font-Size="8" Font-Names="Open Sans">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ chart.PerformCallback(&quot;ShowColumnsChanged&quot;); }" />
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ chart.PerformCallback(&quot;ShowColumnsChanged&quot;); }"></ClientSideEvents>
                                                                    </dx:ASPxComboBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <dxchartsui:WebChartControl ID="HealthChartControl" Width="800" Height="400" ClientInstanceName="chart"
                                                            runat="server" OnCustomCallback="HealthChartControl_CustomCallback" CrosshairEnabled="False" ToolTipEnabled="False">
                                                            <fillstyle><OptionsSerializable>
<cc1:SolidFillOptions></cc1:SolidFillOptions>
</OptionsSerializable>
</fillstyle>
                                                            <legend alignmenthorizontal="Center" alignmentvertical="BottomOutside" direction="RightToLeft"></legend>
                                                            <seriestemplate><ViewSerializable>
<cc1:SideBySideBarSeriesView></cc1:SideBySideBarSeriesView>
</ViewSerializable>
<LabelSerializable>
<cc1:SideBySideBarSeriesLabel LineVisible="True">
<FillStyle><OptionsSerializable>
<cc1:SolidFillOptions></cc1:SolidFillOptions>
</OptionsSerializable>
</FillStyle>
</cc1:SideBySideBarSeriesLabel>
</LabelSerializable>
<PointOptionsSerializable>
<cc1:PointOptions></cc1:PointOptions>
</PointOptionsSerializable>
<LegendPointOptionsSerializable>
<cc1:PointOptions></cc1:PointOptions>
</LegendPointOptionsSerializable>
</seriestemplate>
                                                        </dxchartsui:WebChartControl>
                                                    </td>
                                                </tr>
                                            </table>
                                            <%--  <div class="pivotsplitter">
                                    </div>--%>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="QueryPerformancePanel" Visible="false">

                                        <uc1:QueryDisplayControl runat="server" ID="QueryPerformanceData"></uc1:QueryDisplayControl>
                                    </asp:Panel>
                                </dx:SplitterContentControl>
                            </ContentCollection>
                        </dx:SplitterPane>
                        <dx:SplitterPane MinSize="0" Size="1%" Collapsed="true" PaneStyle-Border-BorderStyle="None"
                            PaneStyle-BackColor="#D6E3F7" Separator-Size="0" Separator-Visible="False">
                            <Separator Size="0px" Visible="False">
                            </Separator>
                            <PaneStyle BackColor="#D6E3F7">
                                <Border BorderStyle="None"></Border>
                            </PaneStyle>
                            <ContentCollection>
                                <dx:SplitterContentControl ID="SplitterContentControl3" runat="server" SupportsDisabledAttribute="True">
                                </dx:SplitterContentControl>
                            </ContentCollection>
                        </dx:SplitterPane>
                    </Panes>
                    <ContentCollection>
                        <dx:SplitterContentControl ID="SplitterContentControl4" runat="server" SupportsDisabledAttribute="True">
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>
            </Panes>

            <ClientSideEvents PaneResized="OnSplitterPaneResized"></ClientSideEvents>

            <Styles>
                <Pane>
                    <Paddings Padding="0" />
                    <Paddings Padding="0px"></Paddings>
                </Pane>
            </Styles>
        </dx:ASPxSplitter>
    </div>
</div>
