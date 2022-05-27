<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsolePageDetails.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsolePageDetails" %>

<style>
    .DetailTable {
        padding: 30px;
        margin-top: 30px;
        background-color: #f3f3f3;
        border-radius: 10px;
    }
    .DetailTable table tr td:first-child {
        width: 160px;
    }
    .DetailTable table tr td {
        padding-top: 7px;
    }
    .DetailTable .Header {
        border-bottom: 1px solid #ccc;
    }
    .DetailTable .Section {
        padding-top: 30px;
        font-weight: bold;
    }
</style>

<asp:Panel ID="pnlNodeDetails" runat="server" Visible="false" EnableViewState="false">
    <p style="font-size:4.2em">
        <%# GetSelectedNodeName() %>
    </p>
    <p style="padding-top:15px">
        <%# GetSelectedNodeUrl() %>
    </p>
    <p style="padding-top:15px">
        <b>Data refreshed on:</b> <%# GetLastRefreshTime() %><br />
        <b>Indexing requested on:</b> <%# GetIndexingRequestTime() %>
    </p>
    <div class="DetailTable">
        <table>
            <tbody>
                <tr>
                    <td class="Header"><b>Coverage</b></td>
                    <td class="Header"><%# GetCoverageMessage() %></td>
                </tr>
                <tr>
                    <td colspan="2" class="Section">Discovery</td>
                </tr>
                <tr>
                    <td>Sitemaps</td>
                    <td><%# GetSitemapMessage() %></td>
                </tr>
                <tr>
                    <td style="vertical-align:top">Referring URLs</td>
                    <td><%# GetReferrersMessage() %></td>
                </tr>
                <tr>
                    <td colspan="2" class="Section">Crawl</td>
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
                    <td><%# GetRobotsTxtMessage() %></td>
                </tr>
                <tr>
                    <td colspan="2" class="Section">Indexing</td>
                </tr>
                <tr>
                    <td>Indexing allowed</td>
                    <td><%# GetIndexingAllowedMessage() %></td>
                </tr>
                <tr>
                    <td>Page fetch</td>
                    <td><%# GetPageFetchAllowedMessage() %></td>
                </tr>
                <tr>
                    <td>Canonical URL</td>
                    <td><%# GetUrlMatchMessage() %></td>
                </tr>
                <%# GetCanonicalUrls() %>
            </tbody>
        </table>
    </div>
    <div class="DetailTable">
        <table>
            <tbody>
                <tr>
                    <td class="Header"><b>Mobile usability</b></td>
                    <td class="Header"><%# GetMobileUsabilityMessage() %></td>
                </tr>
                <%# GetMobileUsabilityIssues() %>
            </tbody>
        </table>
    </div>
    <div class="DetailTable">
        <table>
            <tbody>
                <tr>
                    <td class="Header"><b>Rich results</b></td>
                    <td class="Header"><%# GetRichResultsMessage() %></td>
                </tr>
                <%# GetRichResultsIssues() %>
            </tbody>
        </table>
    </div>
    <div class="DetailTable">
        <table>
            <tbody>
                <tr>
                    <td class="Header"><b>AMP status</b></td>
                    <td class="Header"><%# GetAmpMessage() %></td>
                </tr>
                <tr>
                    <td>Indexing</td>
                    <td><%# GetAmpIndexingState() %></td>
                </tr>
                <tr>
                    <td>Crawl allowed</td>
                    <td><%# GetAmpRobotsTxtMessage() %></td>
                </tr>
                <tr>
                    <td>Last crawl</td>
                    <td><%# GetAmpLastCrawlTime() %></td>
                </tr>
                <tr>
                    <td>Page fetch</td>
                    <td><%# GetAmpPageFetchAllowedMessage() %></td>
                </tr>
                <tr>
                    <td>AMP URL</td>
                    <td><%# GetAmpUrl() %></td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Panel>