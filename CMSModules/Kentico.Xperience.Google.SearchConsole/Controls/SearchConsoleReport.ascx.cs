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

            ltlHeader.Text = $"Report for section {SelectedNode.NodeAliasPath}/%";
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

                if (inspectionStatus == null)
                {
                    reportItems.Add(reportItem);
                    continue;
                }

                // TODO: Populate report from inspection status
                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                reportItem.LastRefresh = inspectionStatus.InspectionResultRequestedOn.ToString();
                reportItem.Coverage = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict;

                var userUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.UserCanonical;
                var googleUrl = inspectUrlIndexResponse.InspectionResult.IndexStatusResult.GoogleCanonical;
                if (userUrl == googleUrl)
                {
                    reportItem.CanonicalMatch = $"{IconSet.Checked("Match")} Match";
                }
                else
                {
                    var tooltip = $"User canonical: {userUrl}, Google canonical: {googleUrl}";
                    reportItem.CanonicalMatch = $"{IconSet.Error(tooltip)} Mismatch";
                }

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
                    var coverage = ValidationHelper.GetString(parameter, String.Empty);
                    return Verdict.GetIcon(coverage);
                case "referrers":
                    var count = ValidationHelper.GetInteger(parameter, 0);
                    if (count > 0)
                    {
                        var inspectionStatusId = ValidationHelper.GetInteger(drv[nameof(ReportItem.InspectionStatusID)], 0);
                        return $"<a href='#' onclick='openReferringUrls({inspectionStatusId})'>{count}</a>";
                    }

                    break;
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

            if (SelectedNode.IsRoot())
            {
                ltlMessage.Text = "Please select a page from the content tree to view the Google Search Console data.";
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