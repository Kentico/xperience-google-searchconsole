using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;

using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    public partial class SearchConsolePropertyPage : CMSPropertiesPage
    {
        private IPageIndexStatusInfoProvider pageIndexStatusInfoProvider;


        private UITreeView InnerTreeView
        {
            get
            {
                return contentTree.FindControl("pnlTree").FindControl("t") as UITreeView;
            }
        }


        private string IndexedPageIcon
        {
            get
            {
                return UIHelper.GetAccessibleIconTag("icon-check-circle", "Indexed", additionalClass: "tn color-green-100");
            }
        }


        private string UnindexedPageIcon
        {
            get
            {
                return UIHelper.GetAccessibleIconTag("icon-times-circle", "Not indexed", additionalClass: "tn color-red-70");
            }
        }


        private string UnknownPageIcon
        {
            get
            {
                return UIHelper.GetAccessibleIconTag("icon-question-circle", "Unknown", additionalClass: "tn color-blue-100");
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            pageIndexStatusInfoProvider = Service.Resolve<IPageIndexStatusInfoProvider>();
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            var accessToken = searchConsoleService.GetAccessToken();
            if (accessToken == null)
            {
                pnlMain.Visible = false;
            }
            else
            {
                InitTreeView();
                InitSelectedNodeDetails();
                InitActions();
            }
        }


        private void InitActions()
        {
            var sectionNodeIds = InnerTreeView.Nodes.Cast<System.Web.UI.WebControls.TreeNode>().Select(n => ValidationHelper.GetInteger(n.Value, 0));
            consoleActions.InitializeNodeData(contentTree.SelectedNodeID, sectionNodeIds, CultureCode);
        }


        private void InitTreeView()
        {
            var url = Request.RawUrl;
            var selectedNode = QueryHelper.GetInteger("selectedNode", 0);
            url = URLHelper.RemoveParameterFromUrl(url, "selectedNode");
            var onClickScript = $"function nodeSelected(nodeId) {{ window.location = '{url}&selectedNode=' + nodeId; }}";
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "nodeSelected", ScriptHelper.GetScript(onClickScript));

            contentTree.NodeTextTemplate = "<span style=\"margin-right:10px\" class=\"ContentTreeItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            contentTree.SelectedNodeTextTemplate = "<span style=\"margin-right:10px\" id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

            if (InnerTreeView != null)
            {
                InnerTreeView.ShowExpandCollapse = false;
                InnerTreeView.TreeNodePopulate += (sender, eventArgs) => SetExpandedSectionIndexedStatus(eventArgs.Node.ChildNodes);
            }

            contentTree.Culture = CultureCode;
            contentTree.Path = Node.NodeAliasPath;
            contentTree.ExpandNodeID = Node.NodeID;
            if (selectedNode > 0)
            {
                contentTree.SelectedNodeID = selectedNode;
            }
            else
            {
                contentTree.SelectedNodeID = Node.NodeID;
            }
        }


        private void InitSelectedNodeDetails()
        {
            if (contentTree.SelectedNodeID == 1)
            {
                // Root node, hide details
                pnlNodeDetails.Visible = false;
                return;
            }

            var url = DocumentURLProvider.GetAbsoluteUrl(contentTree.SelectedNode);
            if (String.IsNullOrEmpty(url))
            {
                // No live site URL, hide details
                pnlNodeDetails.Visible = false;
                return;
            }

            pnlNodeDetails.Visible = true;
            ltlUrl.Text = url;

            var indexedStatus = pageIndexStatusInfoProvider.Get()
                .WhereEquals(nameof(PageIndexStatusInfo.Url), url)
                .TopN(1)
                .TypedResult
                .FirstOrDefault();

            if (indexedStatus == null)
            {
                ltlStatus.Text = "Indexing status for this URL has not yet been retrieved from Google.";
            }
        }


        private void SetExpandedSectionIndexedStatus(System.Web.UI.WebControls.TreeNodeCollection nodes)
        {
            // Update the parent of the section
            if (nodes.Count > 0 && nodes[0].Parent != null)
            {
                SetStatusOfNode(nodes[0].Parent);
            }

            foreach (System.Web.UI.WebControls.TreeNode node in nodes)
            {
                SetStatusOfNode(node);
            }
        }


        private void SetStatusOfNode(System.Web.UI.WebControls.TreeNode node)
        {
            var nodeId = ValidationHelper.GetInteger(node.Value, 0);
            if (nodeId == 1)
            {
                // Root node, skip
                return;
            }

            var page = new TreeProvider().SelectSingleNode(nodeId, CultureCode);
            var url = DocumentURLProvider.GetAbsoluteUrl(page);
            if (String.IsNullOrEmpty(url))
            {
                return;
            }

            var indexedStatus = pageIndexStatusInfoProvider.Get()
                .WhereEquals(nameof(PageIndexStatusInfo.Url), url)
                .TopN(1)
                .TypedResult
                .FirstOrDefault();

            if (indexedStatus == null)
            {
                node.Text += UnknownPageIcon;
                return;
            }

            if (String.IsNullOrEmpty(indexedStatus.LatestUpdate))
            {
                node.Text += UnindexedPageIcon;
                return;
            }

            node.Text += IndexedPageIcon;
        }
    }
}