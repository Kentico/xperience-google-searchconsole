using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using Kentico.Xperience.Google.SearchConsole.Services;

using System;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    public partial class SearchConsoleLayout : CMSPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            /*if (searchConsoleService.GetUserCredential() == null)
            {
                pnlMain.Visible = false;
                btnAuth.Visible = true;
                return;
            }*/

            var selectedNodeId = QueryHelper.GetInteger("selectednodeid", 0);
            var selectedCulture = QueryHelper.GetString("selectedculture", String.Empty);
            if (String.IsNullOrEmpty(selectedCulture))
            {
                selectedCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
            }

            TreeNode selectedNode;
            if (selectedNodeId == 0)
            {
                selectedNode = new TreeProvider().SelectSingleNode(SiteContext.CurrentSiteName, "/", selectedCulture, true, SystemDocumentTypes.Root, false);
            }
            else
            {
                selectedNode = new TreeProvider().SelectSingleNode(selectedNodeId, selectedCulture);
            }

            actionPanel.SelectedNode = selectedNode;
            contentTree.SelectedCulture = selectedCulture;
            contentTree.SelectedNodeID = selectedNodeId;

            if (selectedNode == null)
            {
                messageContainer.InnerHtml = "The selected page doesn't exist in the selected culture.";
                messageContainer.Visible = true;
                pnlActions.Visible = false;
                return;
            }

            consoleDetails.StopProcessing = false;
            consoleDetails.SelectedNode = selectedNode;
            consoleDetails.SelectedCulture = selectedCulture;
            consoleReport.StopProcessing = false;
            consoleReport.SelectedNode = selectedNode;
            consoleReport.SelectedCulture = selectedCulture;

            var url = DocumentURLProvider.GetAbsoluteUrl(selectedNode);
            if (String.IsNullOrEmpty(url))
            {
                actionPanel.AllowIndexSingle = false;
                actionPanel.AllowRefreshSingle = false;
            }

            if (selectedNode.Children.Count == 0)
            {
                actionPanel.AllowIndexSection = false;
                actionPanel.AllowRefreshSection = false;
            }
        }
    }
}