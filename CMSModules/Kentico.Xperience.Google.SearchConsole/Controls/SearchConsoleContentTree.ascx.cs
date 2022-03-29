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

using static Kentico.Xperience.Google.SearchConsole.Pages.SearchConsoleLayout;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsoleContentTree : AbstractUserControl
    {
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;


        private UITreeView InnerTreeView
        {
            get
            {
                return contentTree.FindControl("pnlTree").FindControl("t") as UITreeView;
            }
        }


        public string SelectedCulture
        {
            get;
            set;
        }


        public int SelectedNodeID
        {
            get;
            set;
        }


        public int SelectedMode
        {
            get;
            set;
        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var elementUrl = GetElementUrl();
            var nodeClickScript = $@"
function nodeSelected(nodeId) {{
    window.location = '{elementUrl}?SelectedCulture={SelectedCulture}'
    +'&SelectedMode={SelectedMode}'
    +'&SelectedNodeID='+nodeId;
}}";
            var cultureChangeScript = $@"
function cultureSelected(control) {{
    var culture = control.value;
    window.location = '{elementUrl}?SelectedNodeID={SelectedNodeID}'
    +'&SelectedMode={SelectedMode}'
    +'&SelectedCulture='+culture;
}}";

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "nodeSelected", ScriptHelper.GetScript(nodeClickScript));
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "cultureSelected", ScriptHelper.GetScript(cultureChangeScript));

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            InitTreeView();
        }


        protected void btnModeOverview_Click(object sender, EventArgs e)
        {
            ChangeMode((int)LayoutMode.Overview);
        }


        protected void btnModeReport_Click(object sender, EventArgs e)
        {
            ChangeMode((int)LayoutMode.Report);
        }


        private void ChangeMode(int mode)
        {
            var url = GetElementUrl();
            url = URLHelper.AddParameterToUrl(url, "SelectedMode", mode.ToString());
            url = URLHelper.AddParameterToUrl(url, "SelectedCulture", SelectedCulture);
            url = URLHelper.AddParameterToUrl(url, "SelectedNodeID", SelectedNodeID.ToString());

            URLHelper.Redirect(url);
        }


        private string GetElementUrl()
        {
            return ApplicationUrlHelper.GetElementUrl("Kentico.Xperience.Google.SearchConsole", "GoogleSearchConsole");
        }


        private void InitTreeView()
        {
            contentTree.Culture = SelectedCulture;
            contentTree.NodeTextTemplate = "<span style=\"margin-right:10px;margin-left:5px\" class=\"ContentTreeItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            contentTree.SelectedNodeTextTemplate = "<span style=\"margin-right:10px;margin-left:5px\" id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

            if (InnerTreeView != null)
            {
                InnerTreeView.TreeNodePopulate += (sender, eventArgs) => SetExpandedSectionIndexedStatus(eventArgs.Node.ChildNodes);
            }

            if (SelectedNodeID > 0)
            {
                contentTree.SelectedNodeID = SelectedNodeID;
            }

            if (SelectedMode == (int)SearchConsoleLayout.LayoutMode.Overview)
            {
                btnModeOverview.RemoveCssClass("btn-default");
                btnModeOverview.AddCssClass("btn-primary");
            }
            else if (SelectedMode == (int)SearchConsoleLayout.LayoutMode.Report)
            {
                btnModeReport.RemoveCssClass("btn-default");
                btnModeReport.AddCssClass("btn-primary");
            }

            drpCulture.Value = SelectedCulture;
            drpCulture.UniSelector.OnAfterClientChanged = $"cultureSelected(this);";
        }


        private void SetExpandedSectionIndexedStatus(System.Web.UI.WebControls.TreeNodeCollection nodes)
        {
            foreach (System.Web.UI.WebControls.TreeNode node in nodes)
            {
                var nodeId = ValidationHelper.GetInteger(node.Value, 0);
                var page = new TreeProvider().SelectSingleNode(nodeId, SelectedCulture);
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
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), SelectedCulture)
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