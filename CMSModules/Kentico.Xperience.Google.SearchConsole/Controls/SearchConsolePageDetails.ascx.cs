using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsolePageDetails : InlineUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;
        private ISearchConsoleService searchConsoleService;
        private InspectUrlIndexResponse inspectUrlIndexResponse;
        private UrlInspectionStatusInfo inspectionStatus;


        private int SelectedNodeID
        {
            get
            {
                return QueryHelper.GetInteger("selectedNode", 0);
            }
        }


        private TreeNode SelectedNode
        {
            get
            {
                return new TreeProvider().SelectSingleNode(SelectedNodeID, SelectedCulture);
            }
        }


        private string SelectedCulture
        {
            get
            {
                return ValidationHelper.GetString(Session[SearchConsoleConstants.SESSION_SELECTEDCULTURE], CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName));
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            searchConsoleService = Service.Resolve<ISearchConsoleService>();

            ScriptHelper.RegisterDialogScript(Page);

            Initialize();
            DataBind();
        }


        protected void btnGetSingleStatus_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.GetInspectionResults(new string[] { url }, SelectedCulture);
            if (result.SuccessfulRequests == 1)
            {
                ShowInformation("Request successful, please reoload the page to view the updated information from Google.");
            }
            else
            {
                ShowError("Request failed. Please check the Event Log for more information.");
            }
        }


        protected void btnGetSectionStatus_Click(object sender, EventArgs e)
        {
            var nodesToUpdate = SelectedNode.AllChildren.ToList();
            nodesToUpdate.Add(SelectedNode);

            var urlsToUpdate = nodesToUpdate.Select(n => DocumentURLProvider.GetAbsoluteUrl(n)).Where(url => !String.IsNullOrEmpty(url));
            if (urlsToUpdate.Count() == 0)
            {
                ShowInformation("The pages in the current section do not have live site URLs.");
                return;
            }

            var result = searchConsoleService.GetInspectionResults(urlsToUpdate, SelectedCulture);
            if (result.SuccessfulRequests == urlsToUpdate.Count())
            {
                ShowInformation("Request successful, please reload the page to view the updated information from Google.");
            }
            else
            {
                ShowError($"{result.FailedRequests}/{urlsToUpdate.Count()} requests failed. Please check the Event Log for more information.");
            }
        }


        protected string GetUrlMatchMessage()
        {
            if (inspectUrlIndexResponse == null)
            {
                return "N/A";
            }

            var userUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.UserCanonical;
            var googleUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.GoogleCanonical;
            if (userUrl == googleUrl)
            {
                return $"{IconSet.Checked("Match")} Match";
            }

            var tooltip = $"User canonical: {userUrl}, Google canonical: {googleUrl}";
            return $"{IconSet.Error(tooltip)} Mismatch";
        }


        protected string GetLastRefreshTime()
        {
            return inspectionStatus == null ? "N/A" : inspectionStatus.InspectionResultRequestedOn.ToString();
        }


        protected string GetSelectedNodeName()
        {
            return SelectedNode == null ? String.Empty : SelectedNode.DocumentName;
        }


        protected string GetSelectedNodeUrl()
        {
            if (SelectedNode == null)
            {
                return String.Empty;
            }

            var searchConsoleLink = String.Empty;
            if (inspectUrlIndexResponse != null)
            {
                searchConsoleLink = $"<a style='padding-left:15px' href='{inspectUrlIndexResponse.InspectionResult.InspectionResultLink}' target='_blank'>View in Google Search Console</a>"; 
            }

            var nodeUrl = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            return $"<i>{nodeUrl}</i> {searchConsoleLink}";
        }


        protected string GetCoverageMessage()
        {
            if (inspectUrlIndexResponse == null)
            {
                return $"{Verdict.GetIcon(Verdict.VERDICT_UNSPECIFIED)} Inspection status for this URL has not yet been retrieved from Google.";
            }

            var icon = Verdict.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict);
            return $"{icon} {inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CoverageState}";
        }


        protected string GetSitemapMessage()
        {
            if (inspectUrlIndexResponse != null &&
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Sitemap != null &&
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Sitemap.Count > 0)
            {
                return String.Join("<br/>", inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Sitemap);
            }

            return "N/A";
        }


        protected string GetReferrersMessage()
        {
            if (inspectUrlIndexResponse != null &&
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls != null &&
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count > 0)
            {
                var url = UrlResolver.ResolveUrl($"~/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/ReferringUrlsDialog.aspx?inspectionStatusID={inspectionStatus.PageIndexStatusID}");
                var script = $"function OpenReferringUrls() {{ modalDialog('{url}', 'OpenReferringUrls', '900', '600', null); return false; }}";
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "OpenReferringUrls", script, true);

                return $"<a href='#' onclick='OpenReferringUrls()'>{inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count}</a>";
            }

            return "None detected";
        }


        protected string GetLastCrawlTime()
        {
            if (inspectUrlIndexResponse != null &&
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime != null)
            {
                var dateTime = DateTime.Parse(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime.ToString());
                return dateTime.ToShortDateString();
            }

            return "N/A";
        }


        protected string GetCrawledAsMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CrawledAs))
            {
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CrawledAs)
                {
                    case CrawlingUserAgent.CRAWLING_USER_AGENT_UNSPECIFIED:
                        return "Unknown user agent";
                    case CrawlingUserAgent.DESKTOP:
                        return "Desktop user agent";
                    case CrawlingUserAgent.MOBILE:
                        return "Mobile user agent";
                }
            }

            return "N/A";
        }


        protected string GetRobotsTxtMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState))
            {
                var icon = RobotsTxtState.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState);
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState)
                {
                    case RobotsTxtState.ROBOTS_TXT_STATE_UNSPECIFIED:
                        return $"{icon} The page wasn't fetched or found, or the robots.txt couldn't be reached";
                    case RobotsTxtState.DISALLOWED:
                        return $"{icon} Disallowed";
                    case RobotsTxtState.ALLOWED:
                        return $"{icon} Allowed";
                }
            }

            return "N/A";
        }


        protected string GetPageFetchAllowedMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState))
            {
                var icon = PageFetchState.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState);
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState)
                {
                    case PageFetchState.SUCCESSFUL:
                        return $"{icon} Successful";
                    case PageFetchState.ACCESS_FORBIDDEN:
                        return $"{icon} Access forbidden";
                    case PageFetchState.ACCESS_DENIED:
                        return $"{icon} Access denied";
                    case PageFetchState.SOFT_404:
                        return $"{icon} Soft 404";
                    case PageFetchState.BLOCKED_ROBOTS_TXT:
                        return $"{icon} Blocked by robots.txt";
                    case PageFetchState.NOT_FOUND:
                        return $"{icon} Page not found";
                    case PageFetchState.SERVER_ERROR:
                        return $"{icon} Server error";
                    case PageFetchState.REDIRECT_ERROR:
                        return $"{icon} Redirection error";
                    case PageFetchState.BLOCKED_4XX:
                        return $"{icon} Blocked by 4XX error (not 403 or 404)";
                    case PageFetchState.INTERNAL_CRAWL_ERROR:
                        return $"{icon} Crawler error";
                    case PageFetchState.INVALID_URL:
                        return $"{icon} Invalid URL";
                    case PageFetchState.PAGE_FETCH_STATE_UNSPECIFIED:
                    default:
                        return $"{icon} Unknown";
                }
            }

            return "N/A";
        }


        protected string GetIndexingAllowedMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState))
            {
                var icon = IndexingState.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState);
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState)
                {
                    case IndexingState.BLOCKED_BY_META_TAG:
                        return $"{icon} Indexing not allowed, 'noindex' detected in 'robots' meta tag";
                    case IndexingState.BLOCKED_BY_HTTP_HEADER:
                        return $"{icon} Indexing not allowed, 'noindex' detected in 'X-Robots-Tag' http header";
                    case IndexingState.INDEXING_ALLOWED:
                        return $"{icon} Indexing allowed";
                    case IndexingState.INDEXING_STATE_UNSPECIFIED:
                    default:
                        return $"{icon} Unknown";
                }
            }

            return "N/A";
        }


        private void Initialize()
        {
            if (searchConsoleService.GetUserCredential() == null)
            {
                btnAuth.Visible = true;
                pnlActions.Visible = false;
                pnlNodeDetails.Visible = false;
                return;
            }

            if (SelectedNodeID == 0 ||
                (SelectedNode != null && SelectedNode.IsRoot()))
            {
                pnlActions.Visible = false;
                pnlNodeDetails.Visible = false;
                ltlMessage.Text = "Please select a page from the content tree to view the Google Search Console data.";
                ltlMessage.Visible = true;
                return;
            }

            if (SelectedNode == null)
            {
                pnlActions.Visible = false;
                pnlNodeDetails.Visible = false;
                ltlMessage.Text = "The selected page doesn't exist in the selected culture.";
                ltlMessage.Visible = true;
                return;
            }

            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            if (String.IsNullOrEmpty(url))
            {
                pnlActions.Visible = true;
                pnlNodeDetails.Visible = false;
                btnGetSingleStatus.Visible = false;
                ltlMessage.Text = "The selected page doesn't have a URL, but you can still perform operations on the section.";
                ltlMessage.Visible = true;
                return;
            }

            pnlActions.Visible = true;
            pnlNodeDetails.Visible = true;
            inspectionStatus = urlInspectionStatusInfoProvider.Get()
                .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SelectedCulture)
                .TopN(1)
                .TypedResult
                .FirstOrDefault();

            if (inspectionStatus != null)
            {
                inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
            }
        }
    }
}