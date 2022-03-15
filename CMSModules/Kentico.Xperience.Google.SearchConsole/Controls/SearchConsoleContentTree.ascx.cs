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
                var dropdownValue = drpCulture.Value.ToString();

                // Try to get from query string
                var queryStringValue = QueryHelper.GetString("selectedCulture", String.Empty);
                if (!String.IsNullOrEmpty(queryStringValue))
                {
                    if (String.IsNullOrEmpty(dropdownValue) || queryStringValue.Equals(dropdownValue, StringComparison.OrdinalIgnoreCase))
                    {
                        return queryStringValue;
                    }
                }

                // No query string value
                if (String.IsNullOrEmpty(dropdownValue))
                {
                    dropdownValue = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
                }

                return dropdownValue;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            urlInspectionStatusInfoProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            InitTreeView();
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

            drpCulture.Value = SelectedCulture;
            Session[SearchConsoleConstants.SESSION_SELECTEDCULTURE] = SelectedCulture;
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
                    node.Text += UIHelper.GetAccessibleIconTag("icon-question-circle", "Unknown", additionalClass: "tn color-blue-100");
                    continue;
                }

                var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
                switch (inspectUrlIndexResponse.InspectionResult.IndexStatusResult.Verdict)
                {
                    case Verdict.PASS:
                        node.Text += UIHelper.GetAccessibleIconTag("icon-check-circle", "Valid", additionalClass: "tn color-green-100");
                        break;
                    case Verdict.FAIL:
                        node.Text += UIHelper.GetAccessibleIconTag("icon-times-circle", "Error", additionalClass: "tn color-red-70");
                        break;
                    case Verdict.PARTIAL:
                        node.Text += UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", "Valid with warnings", additionalClass: "tn-color-yellow-100");
                        break;
                    case Verdict.NEUTRAL:
                        node.Text += UIHelper.GetAccessibleIconTag("icon-minus-circle", "Excluded", additionalClass: "tn color-blue-100");
                        break;
                    case Verdict.VERDICT_UNSPECIFIED:
                    default:
                        node.Text += UIHelper.GetAccessibleIconTag("icon-question-circle", "Unknown", additionalClass: "tn color-blue-100");
                        break;
                }
            }
        }
    }
}