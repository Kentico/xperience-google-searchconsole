﻿using CMS.Base.Web.UI;
using CMS.Core;
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
            }
        }


        private void InitTreeView()
        {
            var url = Request.RawUrl;
            var selectedNode = QueryHelper.GetInteger("selectedNode", 0);
            url = URLHelper.RemoveParameterFromUrl(url, "selectedNode");
            var onClickScript = $"function nodeSelected(nodeId) {{ window.location = '{url}&selectedNode=' + nodeId; }}";
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "nodeSelected", ScriptHelper.GetScript(onClickScript));

            contentTree.NodeTextTemplate = "<span class=\"ContentTreeItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            contentTree.SelectedNodeTextTemplate = "<span id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

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


        private void SetExpandedSectionIndexedStatus(System.Web.UI.WebControls.TreeNodeCollection nodes)
        {
            foreach (System.Web.UI.WebControls.TreeNode node in nodes)
            {
                var nodeId = ValidationHelper.GetInteger(node.Value, 0);
                var indexedStatus = pageIndexStatusInfoProvider.Get()
                    .WhereEquals(nameof(PageIndexStatusInfo.NodeID), nodeId)
                    .WhereEquals(nameof(PageIndexStatusInfo.CultureCode), CultureCode)
                    .TypedResult
                    .FirstOrDefault();

                if (indexedStatus == null)
                {
                    node.Text += UnknownPageIcon;
                    continue;
                }
                
                if (String.IsNullOrEmpty(indexedStatus.LatestUpdate))
                {
                    node.Text += UnindexedPageIcon;
                    continue;
                }

                node.Text += IndexedPageIcon;
            }
        }
    }
}