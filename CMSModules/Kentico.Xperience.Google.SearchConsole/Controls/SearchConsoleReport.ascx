<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleReport.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleReport" %>

<%@ Import Namespace="Kentico.Xperience.Google.SearchConsole.Models" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Tagprefix="cms" Tagname="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>

<asp:Panel ID="pnlReport" runat="server" EnableViewState="false">
    <h4>
        <asp:Literal ID="ltlHeader" runat="server" />
    </h4>
    <div class="cms-bootstrap" style="margin-top:30px">
        <cms:UniGrid ID="gridReport" runat="server" ShowExportMenu="true" EnableViewState="false" IsLiveSite="false">
            <GridActions Parameters="Url">
                <ug:Action runat="server" Name="open" OnClick="window.open('{0}');return false;" Caption="Open URL" FontIconClass="icon-arrow-right-top-square" FontIconStyle="default" />
                <ug:Action runat="server" Name="view" CommandArgument="NodeID" Caption="Show overview" FontIconClass="icon-eye" FontIconStyle="allow" />
            </GridActions>
            <GridColumns>
                <ug:Column runat="server" Source="<%# nameof(ReportItem.DocumentName) %>" Caption="Name" Wrap="false" />
                <ug:Column runat="server" Source="##ALL##" Caption="Coverage" ExternalSourceName="coverage" Wrap="false" />
                <ug:Column runat="server" Source="##ALL##" Caption="Mobile usability" ExternalSourceName="mobile" Wrap="false" />
                <ug:Column runat="server" Source="##ALL##" Caption="Rich results" ExternalSourceName="rich" Wrap="false" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.LastRefresh) %>" Caption="Last refresh" Wrap="false" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.LastCrawl) %>" Caption="Last crawl" Wrap="false" />
            </GridColumns>
        </cms:UniGrid>
    </div>
</asp:Panel>