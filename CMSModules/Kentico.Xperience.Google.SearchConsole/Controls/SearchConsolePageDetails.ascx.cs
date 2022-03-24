using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Pages;
using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.Linq;
using System.Text;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsolePageDetails : InlineUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;
        private ISearchConsoleService searchConsoleService;
        private InspectUrlIndexResponse inspectUrlIndexResponse;
        private UrlInspectionStatusInfo inspectionStatus;


        private TreeNode SelectedNode
        {
            get
            {
                return new TreeProvider().SelectSingleNode(SearchConsoleLayout.SelectedNodeID, SearchConsoleLayout.SelectedCulture);
            }
        }


        private SearchConsoleLayout SearchConsoleLayout
        {
            get
            {
                return Page as SearchConsoleLayout;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            searchConsoleService = Service.Resolve<ISearchConsoleService>();
            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();

            ScriptHelper.RegisterDialogScript(Page);
            ScriptHelper.RegisterJQuery(Page);

            Initialize();
            DataBind();
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


        protected string GetAmpMessage()
        {
            if (inspectUrlIndexResponse == null || inspectUrlIndexResponse.InspectionResult.AmpResult == null)
            {
                return $"{Verdict.GetIcon(Verdict.VERDICT_UNSPECIFIED)} AMP status has not been evaluated yet.";
            }

            var icon = Verdict.GetIcon(inspectUrlIndexResponse.InspectionResult.AmpResult.Verdict);
            var message = Verdict.GetMessage(inspectUrlIndexResponse.InspectionResult.AmpResult.Verdict);
            return $"{icon} {message}";
        }


        protected string GetMobileUsabilityMessage()
        {
            if (inspectUrlIndexResponse == null || inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult == null)
            {
                return $"{Verdict.GetIcon(Verdict.VERDICT_UNSPECIFIED)} Mobile usability has not been evaluated yet.";
            }

            var icon = Verdict.GetIcon(inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Verdict);
            var message = Verdict.GetMessage(inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Verdict);
            var issueCount = inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues.Count;
            if (issueCount > 0)
            {
                message = $"{issueCount} issues detected.";
            }
            
            return $"{icon} {message}";
        }


        protected string GetRichResultsMessage()
        {
            if (inspectUrlIndexResponse == null || inspectUrlIndexResponse.InspectionResult.RichResultsResult == null)
            {
                return $"{Verdict.GetIcon(Verdict.VERDICT_UNSPECIFIED)} Rich results have not been evaluated yet.";
            }

            if (inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Count == 0)
            {
                return $"{Verdict.GetIcon(Verdict.VERDICT_UNSPECIFIED)} No items detected";
            }

            var icon = Verdict.GetIcon(inspectUrlIndexResponse.InspectionResult.RichResultsResult.Verdict);
            var itemCount = inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Select(item => item.Items.Count).Sum();
            var issueCount = inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Select(item => item.Items.Select(i => i.Issues.Count).Sum()).Sum();
            
            return $"{icon} {itemCount} items detected, {issueCount} issues";
        }


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


        protected string GetReferrersMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count == 0)
            {
                return "None detected";
            }

            var url = UrlResolver.ResolveUrl($"~/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/ReferringUrlsDialog.aspx?inspectionStatusID={inspectionStatus.PageIndexStatusID}");
            var script = $"function openReferringUrls() {{ modalDialog('{url}', 'openReferringUrls', '900', '600', null); return false; }}";
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "openReferringUrls", script, true);

            return $"<a href='#' onclick='openReferringUrls()'>{inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count}</a>";
        }


        protected string GetLastCrawlTime()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime == null)
            {
                return "N/A";
            }

            var dateTime = DateTime.Parse(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime.ToString());
            return dateTime.ToShortDateString();
        }


        protected string GetCrawledAsMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CrawledAs))
            {
                return "N/A";
            }

            return CrawlingUserAgent.GetMessage(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CrawledAs);
        }


        protected string GetRobotsTxtMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState))
            {
                return "N/A";
            }

            var icon = RobotsTxtState.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState);
            var message = RobotsTxtState.GetMessage(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState);
            return $"{icon} {message}";
        }


        protected string GetPageFetchAllowedMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState))
            {
                return "N/A";
            }

            var icon = PageFetchState.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState);
            var message = PageFetchState.GetMessage(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState);
            return $"{icon} {message}";
        }


        protected string GetIndexingAllowedMessage()
        {
            if (inspectUrlIndexResponse == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState))
            {
                return "N/A";
            }

            var icon = IndexingState.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState);
            var message = IndexingState.GetMessage(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState);
            return $"{icon} {message}";
        }


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
                        var icon = Severity.GetIcon(issue.Severity);
                        return $"{icon} {issue.IssueMessage}";
                    });

                    result.Append($"<tr><td><a href='#' onclick=\"$cmsj('#issues-{itemWithIssues.Name}').toggle()\">{itemWithIssues.Name}</a></td><td style='display:none' id='issues-{itemWithIssues.Name}'>{String.Join("<br/>", issues)}</td></tr>");
                }
            }

            return result.ToString();
        }


        protected string GetMobileUsabilityIssues()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult == null ||
                inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues.Count == 0)
            {
                return String.Empty;
            }

            var result = new StringBuilder();
            foreach (var issue in inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues)
            {
                result.Append($"<tr><td colspan='2'>{IconSet.Error("Error")} {issue.Message}</td></tr>");
            }

            return result.ToString();
        }


        protected string GetAmpUrl()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null)
            {
                return "N/A";
            }

            return inspectUrlIndexResponse.InspectionResult.AmpResult.AmpUrl;
        }


        protected string GetAmpIndexingState()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null)
            {
                return "N/A";
            }

            var icon = AmpIndexingState.GetIcon(inspectUrlIndexResponse.InspectionResult.AmpResult.IndexingState);
            var message = AmpIndexingState.GetMessage(inspectUrlIndexResponse.InspectionResult.AmpResult.IndexingState);

            return $"{icon} {message}";
        }


        protected string GetAmpRobotsTxtMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.AmpResult.RobotsTxtState))
            {
                return "N/A";
            }

            var icon = RobotsTxtState.GetIcon(inspectUrlIndexResponse.InspectionResult.AmpResult.RobotsTxtState);
            var message = RobotsTxtState.GetMessage(inspectUrlIndexResponse.InspectionResult.AmpResult.RobotsTxtState);
            return $"{icon} {message}";
        }


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


        protected string GetAmpPageFetchAllowedMessage()
        {
            if (inspectUrlIndexResponse == null ||
                inspectUrlIndexResponse.InspectionResult.AmpResult == null ||
                String.IsNullOrEmpty(inspectUrlIndexResponse.InspectionResult.AmpResult.PageFetchState))
            {
                return "N/A";
            }

            var icon = PageFetchState.GetIcon(inspectUrlIndexResponse.InspectionResult.AmpResult.PageFetchState);
            var message = PageFetchState.GetMessage(inspectUrlIndexResponse.InspectionResult.AmpResult.PageFetchState);
            return $"{icon} {message}";
        }


        private void Initialize()
        {
            pnlActions.SelectedNode = SelectedNode;

            if (searchConsoleService.GetUserCredential() == null)
            {
                btnAuth.Visible = true;
                pnlActions.Visible = false;
                return;
            }

            var url = String.Empty;
            if (SelectedNode != null)
            {
                url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            }

            if (SearchConsoleLayout.SelectedNodeID == 0 ||
                SelectedNode == null ||
                SelectedNode.IsRoot() ||
                String.IsNullOrEmpty(url))
            {
                return;
            }

            pnlNodeDetails.Visible = true;
            inspectionStatus = urlInspectionStatusInfoProvider.Get()
                .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SearchConsoleLayout.SelectedCulture)
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