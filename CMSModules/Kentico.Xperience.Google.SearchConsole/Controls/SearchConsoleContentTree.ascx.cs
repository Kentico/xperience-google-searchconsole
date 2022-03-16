using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;

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


        private string SelectedCulture
        {
            get
            {
                var defaultCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
                if (IsPostBack)
                {
                    var dropdownValue = drpCulture.Value.ToString();
                    if (String.IsNullOrEmpty(dropdownValue))
                    {
                        dropdownValue = defaultCulture;
                    }

                    return dropdownValue;
                }
                
                var queryStringValue = QueryHelper.GetString("selectedCulture", String.Empty);
                if (!String.IsNullOrEmpty(queryStringValue))
                {
                    return queryStringValue;
                }

                return defaultCulture;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();

            InitTreeView();

            Session[SearchConsoleConstants.SESSION_SELECTEDCULTURE] = SelectedCulture;
            if (!IsPostBack)
            {
                drpCulture.Value = SelectedCulture;
            }
        }


        private void InitTreeView()
        {
            var elementUrl = ApplicationUrlHelper.GetElementUrl("Kentico.Xperience.Google.SearchConsole", "GoogleSearchConsole");
            var onClickScript = $"function nodeSelected(nodeId) {{ window.location = '{elementUrl}?selectedNode='+nodeId+'&selectedCulture={SelectedCulture}'; }}";
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "nodeSelected", ScriptHelper.GetScript(onClickScript));

            contentTree.Culture = SelectedCulture;
            contentTree.NodeTextTemplate = "<span style=\"margin-right:10px;margin-left:5px\" class=\"ContentTreeItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            contentTree.SelectedNodeTextTemplate = "<span style=\"margin-right:10px;margin-left:5px\" id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"nodeSelected(##NODEID##)\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

            if (InnerTreeView != null)
            {
                InnerTreeView.TreeNodePopulate += (sender, eventArgs) => SetExpandedSectionIndexedStatus(eventArgs.Node.ChildNodes);
            }

            var selectedNode = QueryHelper.GetInteger("selectedNode", 0);
            if (selectedNode > 0)
            {
                contentTree.SelectedNodeID = selectedNode;
            }
        }


        private void SetExpandedSectionIndexedStatus(System.Web.UI.WebControls.TreeNodeCollection nodes)
        {
            foreach (System.Web.UI.WebControls.TreeNode node in nodes)
            {
                var nodeId = ValidationHelper.GetInteger(node.Value, 0);
                var page = new TreeProvider().SelectSingleNode(nodeId, contentTree.Culture);
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
                    .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), contentTree.Culture)
                    .TopN(1)
                    .TypedResult
                    .FirstOrDefault();

                if (inspectionStatus == null)
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