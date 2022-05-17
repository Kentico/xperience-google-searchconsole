using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using Kentico.Xperience.Google.SearchConsole.Services;

using System;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    /// <summary>
    /// <para>A custom module UI page which displays a content tree, a UniGrid reporting select Google Search Console
    /// indexing details of a selected page's direct children, the full Google Search Console indexing details of
    /// a selected page, and buttons for performing Google indexing API functions.</para>
    /// <para>If the Google OAuth token doesn't exist in the /App_Data folder, a button is displayed to initiate the
    /// authentication process.</para>
    /// </summary>
    public partial class SearchConsoleLayout : CMSPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            if (searchConsoleService.GetUserCredential() == null)
            {
                pnlMain.Visible = false;
                btnAuth.Visible = true;
                return;
            }

            btnAuth.StopProcessing = true;

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
                ShowInformation("The selected page doesn't exist in the selected culture.");
                pnlActions.Visible = false;
                return;
            }

            consoleDetails.StopProcessing = false;
            consoleDetails.SelectedNode = selectedNode;
            consoleReport.StopProcessing = false;
            consoleReport.SelectedNode = selectedNode;

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

            // Show success messages for ActionPanel buttons
            if (QueryHelper.GetInteger("refreshed", 0) == 1)
            {
                ShowInformation("Refresh successful.");
            }
            if (QueryHelper.GetInteger("indexed", 0) == 1)
            {
                ShowInformation("Indexing requests submitted. Please check Google Search Console or refresh the page status in several days.");
            }
        }
    }
}