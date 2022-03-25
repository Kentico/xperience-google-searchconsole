<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleReport.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleReport" %>
<%@ Import Namespace="Kentico.Xperience.Google.SearchConsole.Models" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register TagPrefix="uc" TagName="ActionPanel" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/ActionPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="OAuthButton" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/OAuthButton.ascx" %>
<%@ Register Tagprefix="cms" Tagname="UniGrid" Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" %>

<div style="padding:30px">
    <uc:OAuthButton ID="btnAuth" runat="server" Visible="false" EnableViewState="false" />
    <asp:Literal ID="ltlMessage" runat="server" Visible="false" />
    <asp:Panel ID="pnlReport" runat="server" Visible="false" EnableViewState="false">
        <h4>
            <asp:Literal ID="ltlHeader" runat="server" />
        </h4>
        <div class="cms-bootstrap" style="margin-top:30px">
            <cms:CMSUpdatePanel UpdateMode="Always" runat="server">
                <ContentTemplate>
                    <cms:UniGrid ID="gridReport" runat="server" ShowExportMenu="true">
                        <GridColumns>
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.CoverageVerdict) %>" Caption="Coverage" ExternalSourceName="coverage" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.LastRefresh) %>" Caption="Last refresh" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.DocumentName) %>" Caption="Name" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.IndexState) %>" Caption="Indexing allowed" ExternalSourceName="index" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.CrawlAllowed) %>" Caption="Crawl allowed" ExternalSourceName="crawl" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.PageFetch) %>" Caption="Page fetch" ExternalSourceName="fetch" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.MobileVerdict) %>" Caption="Mobile usability" ExternalSourceName="mobile" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.RichResultsVerdict) %>" Caption="Rich results" ExternalSourceName="rich" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.AmpVerdict) %>" Caption="Amp status" ExternalSourceName="amp" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.CanonicalUrls) %>" Caption="Canonical URL" ExternalSourceName="canonical" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.ReferringUrlCount) %>" Caption="Referrers" ExternalSourceName="referrers" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.Sitemaps) %>" Caption="Sitemaps" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.LastCrawl) %>" Caption="Last crawl" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.CrawledAs) %>" Caption="Crawled as" />
                            <ug:Column runat="server" Source="<%# nameof(ReportItem.Url) %>" Caption="URL" />
                        </GridColumns>
                    </cms:UniGrid>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </div>
    </asp:Panel>
</div>