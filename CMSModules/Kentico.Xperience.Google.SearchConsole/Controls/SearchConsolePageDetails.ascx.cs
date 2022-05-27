using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.Linq;
using System.Text;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    /// <summary>
    /// A control which displays all data of a page's <see cref="UrlInspectionStatusInfo"/> in tables.
    /// </summary>
    public partial class SearchConsolePageDetails : InlineUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;
        private ISearchConsoleService searchConsoleService;
        private InspectUrlIndexResponse inspectUrlIndexResponse;
        private UrlInspectionStatusInfo inspectionStatus;


        /// <summary>
        /// The node selected by the user.
        /// </summary>
        public TreeNode SelectedNode
        {
            get;
            set;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (StopProcessing)
            {
                return;
            }

            searchConsoleService = Service.Resolve<ISearchConsoleService>();
            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();

            ScriptHelper.RegisterDialogScript(Page);
            ScriptHelper.RegisterJQuery(Page);

            Initialize();
            DataBind();
        }


        /// <summary>
        /// Gets a message indicting whether the user canonical URL and Google canonical URL match.
        /// </summary>
        protected string GetUrlMatchMessage()
        {
            if (inspectUrlIndexResponse == null)
            {
                return "N/A";
            }

            var userUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.UserCanonical;
            var googleUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.GoogleCanonical;
            if (userUrl == null || googleUrl == null)
            {
                return $"{IconSet.Unknown("Unknown")} Unknown";
            }

            if (userUrl == googleUrl)
            {
                return $"{IconSet.Success("Match")} Match";
            }

            return $"{IconSet.Error("Mismatch")} Mismatch";
        }


        /// <summary>
        /// If the canonical URLs don't match, render a table row displaying each URL. Otherwise, display nothing.
        /// </summary>
        protected string GetCanonicalUrls()
        {
            if (inspectUrlIndexResponse == null)
            {
                return "N/A";
            }

            var userUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.UserCanonical;
            var googleUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.GoogleCanonical;
            if (userUrl == null || googleUrl == null)
            {
                return String.Empty;
            }

            if (userUrl == googleUrl)
            {
                return String.Empty;
            }

            return $"<tr><td>User canonical:</td><td>{userUrl}</td></tr><tr><td>Google canonical:</td><td>{googleUrl}</td></tr>";
        }


        /// <summary>
        /// Gets the last time that the Google Search Console inspection results were retrieved via the API.
        /// </summary>
        protected string GetLastRefreshTime()
        {
            return inspectionStatus == null || inspectionStatus.InspectionResultRequestedOn == DateTime.MinValue ? "N/A" : inspectionStatus.InspectionResultRequestedOn.ToString();
        }


        /// <summary>
        /// Gets the last time that page indexing was requested.
        /// </summary>
        protected string GetIndexingRequestTime()
        {
            return inspectionStatus == null || inspectionStatus.IndexingRequestedOn == DateTime.MinValue ? "N/A" : inspectionStatus.IndexingRequestedOn.ToString();
        }


        /// <summary>
        /// Gets the name of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetSelectedNodeName()
        {
            return SelectedNode == null ? String.Empty : SelectedNode.DocumentName;
        }


        /// <summary>
        /// Gets the live site URL of the <see cref="SelectedNode"/> and a direct link to Google Search Console's inspection
        /// results for the URL.
        /// </summary>
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


        /// <summary>
        /// Gets an icon representing the overall verdict for the <see cref="SelectedNode"/> and its coverage state.
        /// </summary>
        protected string GetCoverageMessage()
        {
            if (inspectUrlIndexResponse == null)
            {
                return $"{IconSet.Unknown("Unknown")} Inspection status for this URL has not yet been retrieved from Google.";
            }

            var icon = new Verdict(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict).GetIcon();
            return $"{icon} {inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CoverageState}";
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="SelectedNode"/>'s <see cref="UrlInspectionResult.AmpResult"/>.
        /// </summary>
        protected string GetAmpMessage()
        {
            if (inspectUrlIndexResponse == null || inspectUrlIndexResponse.InspectionResult.AmpResult == null)
            {
                return $"{IconSet.Unknown("Unknown")} No data available.";
            }

            var verdict = new Verdict(inspectUrlIndexResponse.InspectionResult.AmpResult.Verdict);
            return $"{verdict.GetIcon()} {verdict.GetMessage()}";
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="SelectedNode"/>'s <see cref="UrlInspectionResult.MobileUsabilityResult"/>.
        /// If there are mobile usability issues, the count of the issues are displayed.
        /// </summary>
        protected string GetMobileUsabilityMessage()
        {
            if (inspectUrlIndexResponse == null || inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult == null)
            {
                return $"{IconSet.Unknown("Unknown")} Mobile usability has not been evaluated yet.";
            }

            var verdict = new Verdict(inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Verdict);
            var message = verdict.GetMessage();
            if (inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues != null)
            {
                message = $"{inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues.Count} issues detected.";
            }
            
            return $"{verdict.GetIcon()} {message}";
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="SelectedNode"/>'s <see cref="UrlInspectionResult.RichResultsResult"/>.
        /// If there are rich result items detected, the total number of items and issues are displayed.
        /// </summary>
        protected string GetRichResultsMessage()
        {
            if (inspectUrlIndexResponse == null || inspectUrlIndexResponse.InspectionResult.RichResultsResult == null)
            {
                return $"{IconSet.Unknown("Unknown")} No data available.";
            }

            if (inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Count == 0)
            {
                return $"{IconSet.Warning("No items detected")} No items detected";
            }

            var verdict = new Verdict(inspectUrlIndexResponse.InspectionResult.RichResultsResult.Verdict);
            var itemCount = inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Select(item => item.Items.Count).Sum();
            var issueCount = inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Select(item => item.Items.Select(i => i.Issues.Count).Sum()).Sum();
            
            return $"{verdict.GetIcon()} {itemCount} items detected, {issueCount} issues";
        }


        /// <summary>
        /// Displays the sitemaps that the <see cref="SelectedNode"/> is inculded in.
        /// </summary>
        protected string GetSitemapMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Sitemap == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Sitemap.Count == 0)
            {
                return "N/A";
            }

            return String.Join("<br/>", inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Sitemap);
        }


        /// <summary>
        /// Displays up to 5 referring URLs for the <see cref="SelectedNode"/> and if there are more, renders a clickable link
        /// which opens a new dialog window displaying each referring URL.
        /// </summary>
        protected string GetReferrersMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count == 0)
            {
                return "None detected";
            }

            var text = new StringBuilder();
            foreach (var referrer in inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Take(5))
            {
                text.Append(referrer).Append("<br/>");
            }
            
            if (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count > 5)
            {
                var modalUrl = UrlResolver.ResolveUrl($"~/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/ReferringUrlsDialog.aspx?inspectionStatusID={inspectionStatus.PageIndexStatusID}");
                var script = $"function openReferringUrls() {{ modalDialog('{modalUrl}', 'openReferringUrls', '900', '600', null); return false; }}";
                text.Append($"<a href='#' onclick='openReferringUrls()'>Show more...</a>");

                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "openReferringUrls", script, true);
            }

            return text.ToString();
        }


        /// <summary>
        /// Gets the last time that the <see cref="SelectedNode"/> was crawled.
        /// </summary>
        protected string GetLastCrawlTime()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime == null)
            {
                return "N/A";
            }

            return inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime.ToString();
        }


        /// <summary>
        /// Gets the <see cref="CrawlingUserAgent"/> that was used by Google during the last <see cref="SelectedNode"/> page crawl.
        /// </summary>
        protected string GetCrawledAsMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CrawledAs))
            {
                return "N/A";
            }

            return new CrawlingUserAgent(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CrawledAs).GetMessage();
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="RobotsTxtState"/> of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetRobotsTxtMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState))
            {
                return "N/A";
            }

            var state = new RobotsTxtState(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState);
            return $"{state.GetIcon()} {state.GetMessage()}";
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="PageFetchState"/> of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetPageFetchAllowedMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState))
            {
                return "N/A";
            }

            var state = new PageFetchState(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState);
            return $"{state.GetIcon()} {state.GetMessage()}";
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="IndexingState"/> of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetIndexingAllowedMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState))
            {
                return "N/A";
            }

            var state = new IndexingState(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState);
            return $"{state.GetIcon()} {state.GetMessage()}";
        }


        /// <summary>
        /// Gets a new table row (tr element) for each of the <see cref="SelectedNode"/>'s <see cref="Item.Issues"/> as a
        /// clickable link which reveals the item's issues in detail.
        /// </summary>
        protected string GetRichResultsIssues()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.RichResultsResult == null ||
                inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Count == 0 ||
                inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Select(item => item.Items.Count).Sum() == 0)
            {
                return String.Empty;
            }

            var result = new StringBuilder();
            foreach (var detectedItem in inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Where(item => item.Items.Count > 0))
            {
                foreach (var itemWithIssues in detectedItem.Items.Where(item => item.Issues.Count > 0))
                {
                    var issues = itemWithIssues.Issues.Select(issue => {
                        var severity = new Severity(issue.Severity);
                        return $"{severity.GetIcon()} {issue.IssueMessage}";
                    });

                    result.Append($"<tr><td style='vertical-align:top'><a href='#' onclick=\"$cmsj('#issues-{itemWithIssues.Name}').toggle();return false;\">{itemWithIssues.Name}</a></td><td style='display:none' id='issues-{itemWithIssues.Name}'>{String.Join("<br/>", issues)}</td></tr>");
                }
            }

            return result.ToString();
        }


        /// <summary>
        /// Gets a new table row (tr element) for each of the <see cref="SelectedNode"/>'s <see cref="MobileUsabilityInspectionResult.Issues"/>,
        /// including an icon representing the issue's <see cref="Severity"/>.
        /// </summary>
        protected string GetMobileUsabilityIssues()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult == null ||
                inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues == null ||
                inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues.Count == 0)
            {
                return String.Empty;
            }

            var result = new StringBuilder();
            foreach (var issue in inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues)
            {
                result.Append($"<tr><td colspan='2'>{new Severity(issue.Severity).GetIcon()} {issue.Message}</td></tr>");
            }

            return result.ToString();
        }


        /// <summary>
        /// Gets the <see cref="SelectedNode"/>'s AMP URL.
        /// </summary>
        protected string GetAmpUrl()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null)
            {
                return "N/A";
            }

            return inspectUrlIndexResponse.InspectionResult.AmpResult.AmpUrl;
        }


        /// <summary>
        /// Gets an icon and message for the <see cref="AmpIndexingState"/> of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetAmpIndexingState()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null)
            {
                return "N/A";
            }

            var state = new AmpIndexingState(inspectUrlIndexResponse.InspectionResult.AmpResult.IndexingState);
            return $"{state.GetIcon()} {state.GetMessage()}";
        }


        /// <summary>
        /// Gets an icon and message for the AMP <see cref="RobotsTxtState"/> of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetAmpRobotsTxtMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.AmpResult.RobotsTxtState))
            {
                return "N/A";
            }

            var state = new RobotsTxtState(inspectUrlIndexResponse.InspectionResult.AmpResult.RobotsTxtState);
            return $"{state.GetIcon()} {state.GetMessage()}";
        }


        /// <summary>
        /// Gets the last crawl time for the AMP URL of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetAmpLastCrawlTime()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult.LastCrawlTime == null)
            {
                return "N/A";
            }

            var dateTime = DateTime.Parse(inspectUrlIndexResponse.InspectionResult.AmpResult.LastCrawlTime.ToString());
            return dateTime.ToShortDateString();
        }


        /// <summary>
        /// Gets an icon and message for the AMP <see cref="PageFetchState"/> of the <see cref="SelectedNode"/>.
        /// </summary>
        protected string GetAmpPageFetchAllowedMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.AmpResult.PageFetchState))
            {
                return "N/A";
            }

            var state = new PageFetchState(inspectUrlIndexResponse.InspectionResult.AmpResult.PageFetchState);
            return $"{state.GetIcon()} {state.GetMessage()}";
        }


        /// <summary>
        /// Validates the <see cref="SelectedNode"/> and retrieves its <see cref="UrlInspectionStatusInfo"/>.
        /// </summary>
        private void Initialize()
        {
            var url = String.Empty;
            if (SelectedNode != null)
            {
                url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            }

            if (SelectedNode == null ||
                SelectedNode.IsRoot() ||
                String.IsNullOrEmpty(url))
            {
                return;
            }

            pnlNodeDetails.Visible = true;
            inspectionStatus = urlInspectionStatusInfoProvider.Get()
                .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SelectedNode.DocumentCulture)
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