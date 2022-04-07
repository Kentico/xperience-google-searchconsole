using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Modules;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;

using Newtonsoft.Json;

using System;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    /// <summary>
    /// A control which displays the content tree and a site culture selector. When a node is clicked,
    /// the current page is refreshed with the selected node's ID and culture in the query string parameters.
    /// </summary>
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


        /// <summary>
        /// The culture selected by the user.
        /// </summary>
        public string SelectedCulture
        {
            get;
            set;
        }


        /// <summary>
        /// The node ID selected by the user.
        /// </summary>
        public int SelectedNodeID
        {
            get;
            set;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var baseUrl = RequestContext.URL.AbsolutePath;
            var nodeClickScript = $@"
function nodeSelected(nodeId) {{
    window.location = '{baseUrl}?selectedculture={SelectedCulture}&selectednodeid='+nodeId;
}}";
            var cultureChangeScript = $@"
function cultureSelected(control) {{
    var culture = control.value;
    window.location = '{baseUrl}?selectednodeid={SelectedNodeID}&selectedculture='+culture;
}}";

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "nodeSelected", ScriptHelper.GetScript(nodeClickScript));
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "cultureSelected", ScriptHelper.GetScript(cultureChangeScript));

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            InitTreeView();
        }


        /// <summary>
        /// Sets content tree values and event handlers.
        /// </summary>
        private void InitTreeView()
        {
            contentTree.SelectOnlyPublished = true;
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

            drpCulture.Value = SelectedCulture;
            drpCulture.UniSelector.OnAfterClientChanged = $"cultureSelected(this);";
        }


        /// <summary>
        /// The event handler which is triggered when a node is expanded. Gets the <see cref="UrlInspectionStatusInfo"/> for
        /// each new node in the content tree and renders the appropriate icon.
        /// </summary>
        /// <param name="nodes">The new nodes in the content tree that were revealed as a result of expanding a section.</param>
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
                    node.Text += IconSet.Unknown("Unknown");
                    continue;
                }

                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                node.Text += new Verdict(inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict).GetIcon();
            }
        }
    }
}