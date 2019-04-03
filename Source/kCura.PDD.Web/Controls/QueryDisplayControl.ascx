<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueryDisplayControl.ascx.cs"
    Inherits="kCura.PDD.Web.Controls.QueryDisplayControl" %>
<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView.Export" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxSplitter" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v13.2.Web, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraCharts.Web" TagPrefix="dxchartsui" %>
<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraCharts" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v13.2, Version=13.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPanel" TagPrefix="dx" %>
<div>
		
    <dx:aspxgridview id="QueryDataGridView" width="100%" keyfieldname="Id" runat="server" clientinstancename="QueryGrid" autogeneratecolumns="true" >
        <Styles Header-Wrap="true">
            <AlternatingRow Enabled="True" BackColor="#EEEEEE" />
            <Row CssClass="itemListGridCellStale" />
            <SelectedRow BackColor="#CCCCCC" />
            <Cell Border-BorderColor="#D6E3F7">
                <Border BorderColor="#D6E3F7"></Border>
            </Cell>
            <CommandColumn Border-BorderColor="#D6E3F7">
                <Border BorderColor="#D6E3F7"></Border>
            </CommandColumn>
        </Styles>
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
    </dx:aspxgridview>
</div>
