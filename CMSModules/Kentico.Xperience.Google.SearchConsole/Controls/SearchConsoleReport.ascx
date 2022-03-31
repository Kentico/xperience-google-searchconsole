<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleReport.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleReport" %>

<%@ Import Namespace="Kentico.Xperience.Google.SearchConsole.Models" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Tagprefix="cms" Tagname="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>

<asp:Panel ID="pnlReport" runat="server" EnableViewState="false">
    <h4>
        <asp:Literal ID="ltlHeader" runat="server" />
    </h4>
    <div class="cms-bootstrap" style="margin-top:30px">
        <cms:UniGrid ID="gridReport" runat="server" ShowExportMenu="true" EnableViewState="false" IsLiveSite="false" FilterByQueryString="true">
            <GridOptions DisplayFilter="true" FilterLimit="0" FilterPath="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/ReportFilter.ascx" />
            <GridColumns>
                <ug:Column runat="server" Source="<%# nameof(ReportItem.DocumentName) %>" Caption="Name" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.LastRefresh) %>" Caption="Last refresh" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.CoverageVerdict) %>" Caption="Coverage" ExternalSourceName="coverage" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.IndexState) %>" Caption="Indexing allowed" ExternalSourceName="index" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.CrawlState) %>" Caption="Crawl allowed" ExternalSourceName="crawl" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.FetchState) %>" Caption="Page fetch" ExternalSourceName="fetch" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.MobileVerdict) %>" Caption="Mobile usability" ExternalSourceName="mobile" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.RichResultsVerdict) %>" Caption="Rich results" ExternalSourceName="rich" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.AmpVerdict) %>" Caption="Amp status" ExternalSourceName="amp" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.CanonicalUrls) %>" Caption="Canonical URL" ExternalSourceName="canonical" />
                <ug:Column runat="server" Source="##ALL##" Caption="Referrers" ExternalSourceName="referrers" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.LastCrawl) %>" Caption="Last crawl" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.CrawledAs) %>" Caption="Crawled as" />
                <ug:Column runat="server" Source="<%# nameof(ReportItem.Url) %>" Caption="URL" />
            </GridColumns>
        </cms:UniGrid>
    </div>
</asp:Panel>