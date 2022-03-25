using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Models;
using Kentico.Xperience.Google.SearchConsole.Pages;
using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsoleReport : InlineUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;
        private ISearchConsoleService searchConsoleService;


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
            var url = UrlResolver.ResolveUrl("~/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/ReferringUrlsDialog.aspx");
            var script = $"function openReferringUrls(id) {{ modalDialog('{url}?inspectionStatusID='+id, 'OpenReferringUrls', '900', '600', null); return false; }}";
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "openReferringUrls", script, true);

            Initialize();
            PopulateReport();
        }


        private void PopulateReport()
        {
            if (StopProcessing)
            {
                return;
            }

            var path = SelectedNode.NodeAliasPath;
            if (SelectedNode.IsRoot())
            {
                path = String.Empty;
            }
            ltlHeader.Text = $"Report for section {path}/%";

            var reportItems = new List<ReportItem>();
            foreach (var node in SelectedNode.Children)
            {
                var url = DocumentURLProvider.GetAbsoluteUrl(node);
                if (String.IsNullOrEmpty(url))
                {
                    continue;
                }

                var reportItem = new ReportItem
                {
                    Url = url,
                    DocumentName = node.DocumentName
                };
                
                var inspectionStatus = urlInspectionStatusInfoProvider.Get()
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SearchConsoleLayout.SelectedCulture)
                    .TopN(1)
                    .TypedResult
                    .FirstOrDefault();

                if (inspectionStatus == null || String.IsNullOrEmpty(inspectionStatus.LastInspectionResult))
                {
                    reportItems.Add(reportItem);
                    continue;
                }

                // TODO: Sitemaps
                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                reportItem.LastRefresh = inspectionStatus.InspectionResultRequestedOn.ToString();
                reportItem.CoverageVerdict = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict;
                reportItem.IndexState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState;
                reportItem.FetchState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState;
                reportItem.CrawlState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState;

                if (inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult != null)
                {
                    reportItem.MobileVerdict = inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Verdict;
                }

                if (inspectUrlIndexResponse.InspectionResult.RichResultsResult != null)
                {
                    reportItem.RichResultsVerdict = inspectUrlIndexResponse.InspectionResult.RichResultsResult.Verdict;
                }

                if (inspectUrlIndexResponse.InspectionResult.AmpResult != null)
                {
                    reportItem.RichResultsVerdict = inspectUrlIndexResponse.InspectionResult.AmpResult.Verdict;
                }

                reportItem.CanonicalUrls = new ReportCanonicalUrls
                {
                    UserUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.UserCanonical,
                    GoogleUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.GoogleCanonical
                };

                reportItems.Add(reportItem);
            }

            gridReport.OnExternalDataBound += GridReport_OnExternalDataBound;
            gridReport.DataSource = ToDataSet(reportItems);
            gridReport.DataBind();
        }


        private object GridReport_OnExternalDataBound(object sender, string sourceName, object parameter)
        {
            DataRowView drv = parameter as DataRowView;
            switch (sourceName)
            {
                case "coverage":
                    var coverageVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{Verdict.GetIcon(coverageVerdict)} {Verdict.GetMessage(coverageVerdict)}";
                case "index":
                    var indexState = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{IndexingState.GetIcon(indexState)} {IndexingState.GetMessage(indexState)}";
                case "fetch":
                    var fetchState = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{PageFetchState.GetIcon(fetchState)} {PageFetchState.GetMessage(fetchState)}";
                case "crawl":
                    var crawlState = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{RobotsTxtState.GetIcon(crawlState)} {RobotsTxtState.GetMessage(crawlState)}";
                case "mobile":
                    var mobileVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{Verdict.GetIcon(mobileVerdict)} {Verdict.GetMessage(mobileVerdict)}";
                case "rich":
                    var richResultsVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{Verdict.GetIcon(richResultsVerdict)} {Verdict.GetMessage(richResultsVerdict)}";
                case "amp":
                    var ampVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return $"{Verdict.GetIcon(ampVerdict)} {Verdict.GetMessage(ampVerdict)}";
                case "referrers":
                    var count = ValidationHelper.GetInteger(parameter, 0);
                    if (count > 0)
                    {
                        var inspectionStatusId = ValidationHelper.GetInteger(drv[nameof(ReportItem.InspectionStatusID)], 0);
                        return $"<a href='#' onclick='openReferringUrls({inspectionStatusId})'>{count}</a>";
                    }

                    break;
                case "canonical":
                    var reportCanonicalUrls = parameter as ReportCanonicalUrls;
                    if (reportCanonicalUrls == null ||
                        reportCanonicalUrls.UserUrl == null ||
                        reportCanonicalUrls.GoogleUrl == null)
                    {
                        return $"{IconSet.Question("Unknown")} Unknown";
                    }

                    if (reportCanonicalUrls.UserUrl == reportCanonicalUrls.GoogleUrl)
                    {
                        return $"{IconSet.Checked("Match")} Match";
                    }
                    else
                    {
                        var tooltip = $"User canonical: {reportCanonicalUrls.UserUrl}, Google canonical: {reportCanonicalUrls.GoogleUrl}";
                        return $"{IconSet.Error(tooltip)} Mismatch";
                    }
            }
            return parameter;
        }


        private void Initialize()
        {
            if (searchConsoleService.GetUserCredential() == null)
            {
                btnAuth.Visible = true;
                StopProcessing = true;
                return;
            }

            if (SearchConsoleLayout.SelectedNodeID == 0 ||
                SelectedNode == null)
            {
                ltlMessage.Text = "The selected page doesn't exist in the selected culture.";
                ltlMessage.Visible = true;
                StopProcessing = true;
                return;
            }

            pnlReport.Visible = true;
        }


        /// <summary>
        /// Converts a collection of objects into a <see cref="DataSet"/>.
        /// </summary>
        protected DataSet ToDataSet<T>(IList<T> list)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, ColType);
            }

            foreach (T item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            return ds;
        }
    }
}