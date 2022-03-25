using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Modules;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Pages;

using Newtonsoft.Json;

using System;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsoleContentTree : InlineUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;


        private UITreeView InnerTreeView
        {
            get
            {
                return contentTree.FindControl("pnlTree").FindControl("t") as UITreeView;
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

            var elementUrl = ApplicationUrlHelper.GetElementUrl("Kentico.Xperience.Google.SearchConsole", "GoogleSearchConsole");
            var nodeClickScript = $@"
function nodeSelected(nodeId) {{
    window.location = '{elementUrl}?{nameof(SearchConsoleLayout.SelectedCulture)}={SearchConsoleLayout.SelectedCulture}'
    +'&{nameof(SearchConsoleLayout.SelectedMode)}={SearchConsoleLayout.SelectedMode}'
    +'&{nameof(SearchConsoleLayout.SelectedNodeID)}='+nodeId;
}}";
            var modeButtonClickScript = $@"
function modeSelected(mode) {{
    window.location = '{elementUrl}?{nameof(SearchConsoleLayout.SelectedNodeID)}={SearchConsoleLayout.SelectedNodeID}'
    +'&{nameof(SearchConsoleLayout.SelectedCulture)}={SearchConsoleLayout.SelectedCulture}'
    +'&{nameof(SearchConsoleLayout.SelectedMode)}='+mode;
}}";
            var cultureChangeScript = $@"
function cultureSelected(control) {{
    var culture = control.value;
    window.location = '{elementUrl}?{nameof(SearchConsoleLayout.SelectedNodeID)}={SearchConsoleLayout.SelectedNodeID}'
    +'&{nameof(SearchConsoleLayout.SelectedMode)}={SearchConsoleLayout.SelectedMode}'
    +'&{nameof(SearchConsoleLayout.SelectedCulture)}='+culture;
}}";
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "nodeSelected", ScriptHelper.GetScript(nodeClickScript));
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "modeSelected", ScriptHelper.GetScript(modeButtonClickScript));
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "cultureSelected", ScriptHelper.GetScript(cultureChangeScript));

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            InitTreeView();
        }


        private void InitTreeView()
        {
            contentTree.Culture = SearchConsoleLayout.SelectedCulture;
            contentTree.NodeTextTemplate = "<span style=\"margin-right:10px;margin-left:5px\" class=\"ContentTreeItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            contentTree.SelectedNodeTextTemplate = "<span style=\"margin-right:10px;margin-left:5px\" id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

            if (InnerTreeView != null)
            {
                InnerTreeView.TreeNodePopulate += (sender, eventArgs) => SetExpandedSectionIndexedStatus(eventArgs.Node.ChildNodes);
            }

            if (SearchConsoleLayout.SelectedNodeID > 0)
            {
                contentTree.SelectedNodeID = SearchConsoleLayout.SelectedNodeID;
            }

            if (SearchConsoleLayout.SelectedMode == (int)SearchConsoleLayout.LayoutMode.Overview)
            {
                btnModeOverview.RemoveCssClass("btn-default");
                btnModeOverview.AddCssClass("btn-primary");
            }
            else if (SearchConsoleLayout.SelectedMode == (int)SearchConsoleLayout.LayoutMode.Report)
            {
                btnModeReport.RemoveCssClass("btn-default");
                btnModeReport.AddCssClass("btn-primary");
            }

            drpCulture.Value = SearchConsoleLayout.SelectedCulture;
            drpCulture.UniSelector.OnAfterClientChanged = $"cultureSelected(this);";
        }


        private void SetExpandedSectionIndexedStatus(System.Web.UI.WebControls.TreeNodeCollection nodes)
        {
            foreach (System.Web.UI.WebControls.TreeNode node in nodes)
            {
                var nodeId = ValidationHelper.GetInteger(node.Value, 0);
                var page = new TreeProvider().SelectSingleNode(nodeId, SearchConsoleLayout.SelectedCulture);
                if (page == null || page.IsRoot())
                {
                    continue;
                }

                var url = DocumentURLProvider.GetAbsoluteUrl(page);
                if (String.IsNullOrEmpty(url))
                {
                    continue;
                }

                var inspectionStatus = urlInspectionStatusInfoProvider.Get()
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SearchConsoleLayout.SelectedCulture)
                    .TopN(1)
                    .TypedResult
                    .FirstOrDefault();

                if (inspectionStatus == null || String.IsNullOrEmpty(inspectionStatus.LastInspectionResult))
                {
                    node.Text += IconSet.Question("Unknown");
                    continue;
                }

                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                node.Text += Verdict.GetIcon(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict);
            }
        }
    }
}