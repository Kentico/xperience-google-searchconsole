using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Models;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    /// <summary>
    /// A control which displays select <see cref="UrlInspectionStatusInfo"/> details for a section of the content tree. 
    /// </summary>
    public partial class SearchConsoleReport : AbstractUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;
        private List<ReportItem> currentReportItems;


        /// <summary>
        /// The node selected by the user, whose direct children should be displayed.
        /// </summary>
        public CMS.DocumentEngine.TreeNode SelectedNode
        {
            get;
            set;
        }


        /// <summary>
        /// The underlying UniGrid control.
        /// </summary>
        public UniGrid GridReport
        {
            get
            {
                return gridReport;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (StopProcessing)
            {
                pnlReport.Visible = false;
                return;
            }

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();

            ScriptHelper.RegisterDialogScript(Page);
            var url = UrlResolver.ResolveUrl("~/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/ReferringUrlsDialog.aspx");
            var script = $"function openReferringUrls(id) {{ modalDialog('{url}?inspectionStatusID='+id, 'OpenReferringUrls', '900', '600', null); return false; }}";
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "openReferringUrls", script, true);

            InitializeReportGrid();
        }


        /// <summary>
        /// Sets the report header and registers UniGrid event handlers.
        /// </summary>
        private void InitializeReportGrid()
        {
            ltlHeader.Text = $"Report for {SelectedNode.NodeAliasPath} section";

            gridReport.GridView.Sorting += GridView_Sorting;
            gridReport.OnAction += GridReport_OnAction;
            gridReport.OnDataReload += GridReport_OnDataReload;
            gridReport.OnExternalDataBound += GridReport_OnExternalDataBound;
        }


        /// <summary>
        /// Gets a list of <see cref="ReportItem"/>s for displaying in the report.
        /// </summary>
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
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SelectedNode.DocumentCulture)
                    .TopN(1)
                    .TypedResult
                    .FirstOrDefault();

                if (inspectionStatus == null || String.IsNullOrEmpty(inspectionStatus.LastInspectionResult))
                {
                    reportItems.Add(reportItem);
                    continue;
                }

                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                reportItem.PageIndexStatusID = inspectionStatus.PageIndexStatusID;
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
                        reportItem.MobileIssues = inspectUrlIndexResponse.InspectionResult.MobileUsabilityResult.Issues;
                    }
                }

                if (inspectUrlIndexResponse.InspectionResult.RichResultsResult != null)
                {
                    List<RichResultsIssue> richIssues = new List<RichResultsIssue>();
                    foreach (var detectedItem in inspectUrlIndexResponse.InspectionResult.RichResultsResult.DetectedItems.Where(item => item.Items.Count > 0))
                    foreach (var itemWithIssues in detectedItem.Items.Where(item => item.Issues.Count > 0))
                    foreach (var issue in itemWithIssues.Issues)
                    {
                        // Format issue message to include the detected item
                        issue.IssueMessage = $"{itemWithIssues.Name}: {issue.IssueMessage}";
                        richIssues.Add(issue);
                        
                    }

                    reportItem.RichResultsVerdict = inspectUrlIndexResponse.InspectionResult.RichResultsResult.Verdict;
                    reportItem.RichIssues = richIssues;
                }

                if (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime != null)
                {
                    reportItem.LastCrawl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.LastCrawlTime.ToString();
                }

                reportItems.Add(reportItem);
            }

            return reportItems;
        }


        /// <summary>
        /// Converts a collection of objects into a <see cref="DataSet"/>. Also ensures that <see cref="ReportItem.NodeID"/> is
        /// the first column of the DataSet for mass action support.
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

                var col = t.Columns.Add(propInfo.Name, ColType);
                if (col.ColumnName.Equals(nameof(ReportItem.NodeID), StringComparison.InvariantCultureIgnoreCase))
                {
                    col.SetOrdinal(0);
                }
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


        /// <summary>
        /// Event handler for the UniGrid's individual row actions.
        /// </summary>
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


        /// <summary>
        /// Event handler for the UniGrid's column sorting. As standard SQL sorting can't be applied to the <see cref="currentReportItems"/>,
        /// sorting is manually applied in this event.
        /// </summary>
        private void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            // Inverse the current sort and store in session
            var sessionKey = $"REPORT_{e.SortExpression}_ISASCENDING";
            var columnSortIsAscending = ValidationHelper.GetBoolean(Session[sessionKey], true);
            columnSortIsAscending = !columnSortIsAscending;
            Session[sessionKey] = columnSortIsAscending;

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


        /// <summary>
        /// Gets the data to render in the UniGrid and saves the data in <see cref="currentReportItems"/> to be sorted later. As the
        /// UniGrid doesn't utilize SQL data collection, all parameters are ignored.
        /// </summary>
        private DataSet GridReport_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
        {
            if (StopProcessing)
            {
                return null;
            }

            var data = GetReportData();
            if (data.Count == 0)
            {
                StopProcessing = true;
                pnlReport.Visible = false;
                return null;
            }

            currentReportItems = data;

            return ToDataSet(data);
        }


        /// <summary>
        /// Event handler for the rendering of individual UniGrid values.
        /// </summary>
        private object GridReport_OnExternalDataBound(object sender, string sourceName, object parameter)
        {
            var drv = parameter as DataRowView;
            switch (sourceName)
            {
                case "coverage":
                    var coverageState = ValidationHelper.GetString(drv[nameof(ReportItem.CoverageState)], String.Empty);
                    var verdict = new Verdict(ValidationHelper.GetString(drv[nameof(ReportItem.VerdictState)], Verdict.VERDICT_UNSPECIFIED));
                    if (verdict.constantValue == Verdict.PASS)
                    {
                        return verdict.GetIcon();
                    }

                    return $"{verdict.GetIcon()} {coverageState}";
                case "mobile":
                    var mobileIssues = drv[nameof(ReportItem.MobileIssues)] as IEnumerable<MobileUsabilityIssue>;
                    var mobileVerdict = new Verdict(ValidationHelper.GetString(drv[nameof(ReportItem.MobileVerdict)], Verdict.VERDICT_UNSPECIFIED));
                    if (mobileIssues == null || mobileIssues.Count() == 0)
                    {
                        return mobileVerdict.GetIcon();
                    }

                    var mobileIssueText = new StringBuilder();
                    foreach (var issue in mobileIssues)
                    {
                        mobileIssueText.Append(new Severity(issue.Severity).GetIcon())
                            .Append(" ")
                            .Append(issue.Message)
                            .Append("<br/>");
                    }
                    return mobileIssueText;
                    
                case "rich":
                    var richIssues = drv[nameof(ReportItem.RichIssues)] as IEnumerable<RichResultsIssue>;
                    var richResultsVerdict = new Verdict(ValidationHelper.GetString(drv[nameof(ReportItem.RichResultsVerdict)], Verdict.VERDICT_UNSPECIFIED));
                    if (richIssues == null || richIssues.Count() == 0)
                    {
                        return richResultsVerdict.GetIcon();
                    }

                    var richIssueText = new StringBuilder();
                    foreach (var issue in richIssues)
                    {
                        richIssueText.Append(new Severity(issue.Severity).GetIcon())
                            .Append(" ")
                            .Append(issue.IssueMessage)
                            .Append("<br/>");
                    }
                    return richIssueText.ToString();
            }
            return parameter;
        }
    }
}