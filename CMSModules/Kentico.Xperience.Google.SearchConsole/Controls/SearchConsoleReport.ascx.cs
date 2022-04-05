using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Models;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsoleReport : AbstractUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;
        private List<ReportItem> currentReportItems;


        public CMS.DocumentEngine.TreeNode SelectedNode
        {
            get;
            set;
        }


        public string SelectedCulture
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

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();

            ScriptHelper.RegisterDialogScript(Page);
            var url = UrlResolver.ResolveUrl("~/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/ReferringUrlsDialog.aspx");
            var script = $"function openReferringUrls(id) {{ modalDialog('{url}?inspectionStatusID='+id, 'OpenReferringUrls', '900', '600', null); return false; }}";
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "openReferringUrls", script, true);

            InitializeReportGrid();
        }


        private void InitializeReportGrid()
        {
            var path = SelectedNode.NodeAliasPath;
            if (SelectedNode.IsRoot())
            {
                path = String.Empty;
            }
            ltlHeader.Text = $"Report for section {path}/%";

            gridReport.GridView.Sorting += GridView_Sorting;
            gridReport.OnAction += GridReport_OnAction;
            gridReport.OnDataReload += GridReport_OnDataReload;
            gridReport.OnExternalDataBound += GridReport_OnExternalDataBound;
        }


        private List<ReportItem> GetReportData()
        {
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
                    NodeID = node.NodeID,
                    DocumentName = node.DocumentName
                };

                var inspectionStatus = urlInspectionStatusInfoProvider.Get()
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SelectedCulture)
                    .TopN(1)
                    .TypedResult
                    .FirstOrDefault();

                if (inspectionStatus == null || String.IsNullOrEmpty(inspectionStatus.LastInspectionResult))
                {
                    reportItems.Add(reportItem);
                    continue;
                }

                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                reportItem.InspectionStatusID = inspectionStatus.PageIndexStatusID;
                reportItem.VerdictState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict;
                reportItem.CoverageState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.CoverageState;
                
                if (inspectionStatus.InspectionResultRequestedOn > DateTime.MinValue)
                {
                    reportItem.LastRefresh = inspectionStatus.InspectionResultRequestedOn.ToString();
                }

                if (inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult != null)
                {
                    reportItem.MobileVerdict = inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Verdict;
                    if (inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues != null &&
                        inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues.Count > 0)
                    {
                        reportItem.MobileIssues = String.Join(", ", inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues.Select(i => i.Message));
                    }
                }

                if (inspectUrlIndexResponse.InspectionResult.RichResultsResult != null)
                {
                    List<string> richIssues = new List<string>();
                    foreach (var detectedItem in inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Where(item => item.Items.Count > 0))
                    {
                        foreach (var itemWithIssues in detectedItem.Items.Where(item => item.Issues.Count > 0))
                        {
                            richIssues.AddRange(itemWithIssues.Issues.Select(issue => issue.IssueMessage));
                        }
                    }

                    reportItem.RichResultsVerdict = inspectUrlIndexResponse.InspectionResult.RichResultsResult.Verdict;
                    reportItem.RichIssues = String.Join(", ", richIssues);
                }

                if (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime != null)
                {
                    reportItem.LastCrawl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime.ToString();
                }

                reportItem.CanonicalUrls = new ReportCanonicalUrls
                {
                    UserUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.UserCanonical,
                    GoogleUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.GoogleCanonical
                };

                reportItems.Add(reportItem);
            }

            return reportItems;
        }


        /// <summary>
        /// Converts a collection of objects into a <see cref="DataSet"/>.
        /// </summary>
        private DataSet ToDataSet<T>(IList<T> list)
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


        private void GridReport_OnAction(string actionName, object actionArgument)
        {
            switch (actionName)
            {
                case "view":
                    var nodeId = ValidationHelper.GetString(actionArgument, String.Empty);
                    if (String.IsNullOrEmpty(nodeId))
                    {
                        return;
                    }

                    var url = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "selectednodeid", nodeId);
                    URLHelper.Redirect(url);
                    break;
            }
        }


        private void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            var columnSortIsAscending = ValidationHelper.GetBoolean(Session[$"REPORT_{e.SortExpression}_ISASCENDING"], true);
            columnSortIsAscending = !columnSortIsAscending;
            Session[$"REPORT_{e.SortExpression}_ISASCENDING"] = columnSortIsAscending;

            if (columnSortIsAscending)
            {
                currentReportItems = currentReportItems.OrderBy(item => item.GetType().GetProperty(e.SortExpression).GetValue(item)).ToList();
            }
            else
            {
                currentReportItems = currentReportItems.OrderByDescending(item => item.GetType().GetProperty(e.SortExpression).GetValue(item)).ToList();
            }
            

            gridReport.GridView.DataSource = ToDataSet(currentReportItems);
            gridReport.GridView.DataBind();
        }


        private DataSet GridReport_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
        {
            if (StopProcessing)
            {
                return null;
            }

            var data = GetReportData();
            if (data.Count == 0)
            {
                pnlReport.Visible = false;
                return null;
            }

            currentReportItems = data;

            return ToDataSet(data);
        }


        private object GridReport_OnExternalDataBound(object sender, string sourceName, object parameter)
        {
            var drv = parameter as DataRowView;
            switch (sourceName)
            {
                case "coverage":
                    var coverageState = ValidationHelper.GetString(drv[nameof(ReportItem.CoverageState)], String.Empty);
                    var verdictState = ValidationHelper.GetString(drv[nameof(ReportItem.VerdictState)], String.Empty);
                    if (verdictState == Verdict.PASS)
                    {
                        return Verdict.GetIcon(verdictState);
                    }

                    return $"{Verdict.GetIcon(verdictState)} {coverageState}";
                case "mobile":
                    var mobileVerdict = ValidationHelper.GetString(drv[nameof(ReportItem.MobileVerdict)], String.Empty);
                    if (mobileVerdict == Verdict.PARTIAL || mobileVerdict == Verdict.FAIL)
                    {
                        var issues = ValidationHelper.GetString(drv[nameof(ReportItem.MobileIssues)], String.Empty);
                        return $"{Verdict.GetIcon(mobileVerdict)} {issues}";
                    }

                    return Verdict.GetIcon(mobileVerdict);
                case "rich":
                    var richResultsVerdict = ValidationHelper.GetString(drv[nameof(ReportItem.RichResultsVerdict)], String.Empty);
                    if (richResultsVerdict == Verdict.PARTIAL || richResultsVerdict == Verdict.FAIL)
                    {
                        var issues = ValidationHelper.GetString(drv[nameof(ReportItem.RichIssues)], String.Empty);
                        return $"{Verdict.GetIcon(richResultsVerdict)} {issues}";
                    }

                    return Verdict.GetIcon(richResultsVerdict);
                case "canonical":
                    var reportCanonicalUrls = parameter as ReportCanonicalUrls;
                    if (reportCanonicalUrls == null ||
                        reportCanonicalUrls.UserUrl == null ||
                        reportCanonicalUrls.GoogleUrl == null)
                    {
                        return IconSet.Question("Unknown");
                    }

                    if (reportCanonicalUrls.UserUrl == reportCanonicalUrls.GoogleUrl)
                    {
                        return IconSet.Checked("Match");
                    }
                    else
                    {
                        var tooltip = $"User canonical: {reportCanonicalUrls.UserUrl}, Google canonical: {reportCanonicalUrls.GoogleUrl}";
                        return IconSet.Error(tooltip);
                    }
            }
            return parameter;
        }
    }
}