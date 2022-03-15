<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsolePageDetails.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsolePageDetails" %>
<%@ Register TagPrefix="uc" TagName="OAuthButton" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/OAuthButton.ascx" %>

<style>
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
    .CoverageTable .header {
        border-bottom: 1px solid #ccc;
    }
    .CoverageTable .section {
        padding-top: 30px;
        font-weight: bold;
    }
</style>

<div style="padding:30px">
    <uc:OAuthButton ID="btnAuth" runat="server" Visible="false" />
    <asp:Panel ID="pnlActions" runat="server">
        <asp:Button ID="btnGetSingleStatus" runat="server" CssClass="btn btn-default" Text="Get selected page status" OnClick="btnGetSingleStatus_Click" />
        <asp:Button ID="btnGetSectionStatus" runat="server" CssClass="btn btn-default" Text="Get section status" OnClick="btnGetSectionStatus_Click" />
    </asp:Panel>
    <asp:Panel ID="pnlNodeDetails" runat="server">
        <div style="padding-top:50px">
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
                            <td class="header"><b>Coverage</b></td>
                            <td class="header"><%# GetCoverageMessage() %></td>
                        </tr>
                        <tr>
                            <td class="section">Discovery</td>
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
                            <td class="section">Crawl</td>
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
        </div>
    </asp:Panel>
</div>

