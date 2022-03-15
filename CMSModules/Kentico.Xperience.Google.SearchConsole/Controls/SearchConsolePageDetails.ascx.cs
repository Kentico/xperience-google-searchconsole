using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;

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
                return new TreeProvider().SelectSingleNode(SelectedNodeID, "en-US");
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            searchConsoleService = Service.Resolve<ISearchConsoleService>();
            Initialize();
            DataBind();
        }


        protected void btnGetSingleStatus_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.GetInspectionResults(new string[] { url }, "en-US");
            if (result.SucessfulRequests == 1)
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

            var result = searchConsoleService.GetInspectionResults(urlsToUpdate, "en-US");
            if (result.Errors.Count == 0)
            {
                ShowInformation("Request successful, please reoload the page to view the updated information from Google.");
            }
            else
            {
                ShowError($"{result.FailedRequests}/{urlsToUpdate.Count()} requests failed. Please check the Event Log for more information.");
            }
        }

        protected string GetSelectedNodeName()
        {
            return SelectedNode == null ? String.Empty : SelectedNode.DocumentName;
        }


        protected string GetSelectedNodeUrl()
        {
            return SelectedNode == null ? String.Empty : DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
        }


        protected string GetCoverageMessage()
        {
            return inspectUrlIndexResponse == null ? "Inspection status for this URL has not yet been retrieved from Google." : inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CoverageState;
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
                return String.Join("<br/>", inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls);
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


        protected string GetCrawlAllowedMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState))
            {
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState)
                {
                    case RobotsTxtState.ROBOTS_TXT_STATE_UNSPECIFIED:
                        return "The page wasn't fetched or found, or the robots.txt couldn't be reached";
                    case RobotsTxtState.DISALLOWED:
                        return "Disallowed";
                    case RobotsTxtState.ALLOWED:
                        return "Allowed";
                }
            }

            return "N/A";
        }


        protected string GetPageFetchAllowedMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState))
            {
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState)
                {
                    case PageFetchState.SUCCESSFUL:
                        return "Successful";
                    case PageFetchState.ACCESS_FORBIDDEN:
                        return "Access forbidden";
                    case PageFetchState.ACCESS_DENIED:
                        return "Access denied";
                    case PageFetchState.SOFT_404:
                        return "Soft 404";
                    case PageFetchState.BLOCKED_ROBOTS_TXT:
                        return "Blocked by robots.txt";
                    case PageFetchState.NOT_FOUND:
                        return "Page not found";
                    case PageFetchState.SERVER_ERROR:
                        return "Server error";
                    case PageFetchState.REDIRECT_ERROR:
                        return "Redirection error";
                    case PageFetchState.BLOCKED_4XX:
                        return "Blocked by 4XX error (not 403 or 404)";
                    case PageFetchState.INTERNAL_CRAWL_ERROR:
                        return "Crawler error";
                    case PageFetchState.INVALID_URL:
                        return "Invalid URL";
                    case PageFetchState.PAGE_FETCH_STATE_UNSPECIFIED:
                    default:
                        return "Unknown";
                }
            }

            return "N/A";
        }


        protected string GetIndexingAllowedMessage()
        {
            if (inspectUrlIndexResponse != null &&
                !String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState))
            {
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState)
                {
                    case IndexingState.BLOCKED_BY_META_TAG:
                        return "Indexing not allowed, 'noindex' detected in 'robots' meta tag";
                    case IndexingState.BLOCKED_BY_HTTP_HEADER:
                        return "Indexing not allowed, 'noindex' detected in 'X-Robots-Tag' http header";
                    case IndexingState.INDEXING_ALLOWED:
                        return "Indexing allowed";
                    case IndexingState.INDEXING_STATE_UNSPECIFIED:
                    default:
                        return "Unknown";
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

            if (SelectedNodeID == 0 || SelectedNode.IsRoot())
            {
                pnlActions.Visible = false;
                pnlNodeDetails.Visible = false;
                return;
            }

            if (String.IsNullOrEmpty(DocumentURLProvider.GetAbsoluteUrl(SelectedNode)))
            {
                // Selected node has no live site URL
                pnlNodeDetails.Visible = false;
                btnGetSingleStatus.Visible = false;
                return;
            }

            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            if (String.IsNullOrEmpty(url))
            {
                // No live site URL, hide details
                pnlNodeDetails.Visible = false;
                return;
            }

            pnlNodeDetails.Visible = true;
            var inspectionStatus = urlInspectionStatusInfoProvider.Get()
                .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), "en-US")
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