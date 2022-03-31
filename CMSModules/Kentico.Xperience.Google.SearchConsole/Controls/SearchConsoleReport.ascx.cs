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
                reportItem.CoverageVerdict = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict;
                reportItem.IndexState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.IndexingState;
                reportItem.FetchState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.PageFetchState;
                reportItem.CrawlState = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.RobotsTxtState;

                if (inspectionStatus.InspectionResultRequestedOn > DateTime.MinValue)
                {
                    reportItem.LastRefresh = inspectionStatus.InspectionResultRequestedOn.ToString();
                }

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

                if (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls != null)
                {
                    reportItem.ReferringUrlCount = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls.Count;
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
            if (gridReport.CustomFilter == null || !(gridReport.CustomFilter is ReportFilter))
            {
                return ToDataSet(data);
            }

            var filter = gridReport.CustomFilter as ReportFilter;
            data = filter.Apply(data);

            currentReportItems = data;
            return ToDataSet(data);
        }


        private object GridReport_OnExternalDataBound(object sender, string sourceName, object parameter)
        {
            switch (sourceName)
            {
                case "coverage":
                    var coverageVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return Verdict.GetIcon(coverageVerdict);
                case "index":
                    var indexState = ValidationHelper.GetString(parameter, String.Empty);
                    return IndexingState.GetIcon(indexState);
                case "fetch":
                    var fetchState = ValidationHelper.GetString(parameter, String.Empty);
                    return PageFetchState.GetIcon(fetchState);
                case "crawl":
                    var crawlState = ValidationHelper.GetString(parameter, String.Empty);
                    return RobotsTxtState.GetIcon(crawlState);
                case "mobile":
                    var mobileVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return Verdict.GetIcon(mobileVerdict);
                case "rich":
                    var richResultsVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return Verdict.GetIcon(richResultsVerdict);
                case "amp":
                    var ampVerdict = ValidationHelper.GetString(parameter, String.Empty);
                    return Verdict.GetIcon(ampVerdict);
                case "referrers":
                    var drv = parameter as DataRowView;
                    var count = ValidationHelper.GetInteger(drv[nameof(ReportItem.ReferringUrlCount)], 0);
                    if (count == 0)
                    {
                        return 0;
                    }

                    var inspectionStatusId = ValidationHelper.GetInteger(drv[nameof(ReportItem.InspectionStatusID)], 0);
                    return $"<a href='#' onclick='openReferringUrls({inspectionStatusId})'>{count}</a>";
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