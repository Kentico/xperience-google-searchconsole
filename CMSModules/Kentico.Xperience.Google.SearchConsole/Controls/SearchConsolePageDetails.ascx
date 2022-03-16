<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsolePageDetails.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsolePageDetails" %>
<%@ Register TagPrefix="uc" TagName="OAuthButton" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/OAuthButton.ascx" %>

<style>
    .ActionPanel{
        padding-top: 10px;
        padding-bottom: 50px;
    }
    .CoverageTable {
        padding: 30px;
        margin-top: 30px;
        background-color: #f3f3f3;
        border-radius: 10px;
    }
    .CoverageTable table tr td:first-child {
        width: 160px;
    }
    .CoverageTable table tr td {
        padding-top: 7px;
    }
    .CoverageTable .Header {
        border-bottom: 1px solid #ccc;
    }
    .CoverageTable .Section {
        padding-top: 30px;
        font-weight: bold;
    }
</style>

<div style="padding:30px">
    <uc:OAuthButton ID="btnAuth" runat="server" Visible="false" />
    <asp:Literal ID="ltlMessage" runat="server" EnableViewState="false" Visible="false" />
    <asp:Panel ID="pnlActions" runat="server">
        <div class="ActionPanel">
            <asp:Button ID="btnGetSingleStatus" runat="server" CssClass="btn btn-default" Text="Get selected page status" OnClick="btnGetSingleStatus_Click" />
            <asp:Button ID="btnGetSectionStatus" runat="server" CssClass="btn btn-default" Text="Get section status" OnClick="btnGetSectionStatus_Click" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlNodeDetails" runat="server">
        <p style="font-size:4.2em">
            <%# GetSelectedNodeName() %>
        </p>
        <p style="padding-top:20px">
            <%# GetSelectedNodeUrl() %>
        </p>
        <div class="CoverageTable">
            <table>
                <tbody>
                    <tr>
                        <td class="Header"><b>Coverage</b></td>
                        <td class="Header"><%# GetCoverageMessage() %></td>
                    </tr>
                    <tr>
                        <td class="Section">Discovery</td>
                    </tr>
                    <tr>
                        <td>Sitemaps</td>
                        <td><%# GetSitemapMessage() %></td>
                    </tr>
                    <tr>
                        <td>Referring URLs</td>
                        <td><%# GetReferrersMessage() %></td>
                    </tr>
                    <tr>
                        <td class="Section">Crawl</td>
                    </tr>
                    <tr>
                        <td>Last crawl</td>
                        <td><%# GetLastCrawlTime() %></td>
                    </tr>
                    <tr>
                        <td>Crawled as</td>
                        <td><%# GetCrawledAsMessage() %></td>
                    </tr>
                    <tr>
                        <td>Crawl allowed</td>
                        <td><%# GetCrawlAllowedMessage() %></td>
                    </tr>
                    <tr>
                        <td>Page fetch</td>
                        <td><%# GetPageFetchAllowedMessage() %></td>
                    </tr>
                    <tr>
                        <td>Indexing allowed</td>
                        <td><%# GetIndexingAllowedMessage() %></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </asp:Panel>
</div>