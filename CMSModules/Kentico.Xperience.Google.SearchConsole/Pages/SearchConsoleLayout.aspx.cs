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
        public LayoutMode SelectedMode
        {
            get
            {
                var modeId = QueryHelper.GetString("selectedmode", "0");
                LayoutMode selectedMode;
                if (Enum.TryParse(modeId, out selectedMode))
                {
                    return selectedMode;
                }

                return LayoutMode.Overview;
            }
        }


        public int SelectedNodeID
        {
            get
            {
                return QueryHelper.GetInteger("selectednodeid", 0);
            }
        }


        public string SelectedCulture
        {
            get
            {
                var culture = QueryHelper.GetString("selectedculture", String.Empty);
                if (String.IsNullOrEmpty(culture))
                {
                    culture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
                }

                return culture;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            if (searchConsoleService.GetUserCredential() == null)
            {
                pnlMain.Visible = false;
                btnAuth.Visible = true;
                consoleDetails.StopProcessing = true;
                consoleReport.StopProcessing = true;
                return;
            }

            TreeNode selectedNode;
            if (SelectedNodeID == 0)
            {
                selectedNode = new TreeProvider().SelectSingleNode(SiteContext.CurrentSiteName, "/", SelectedCulture, true, SystemDocumentTypes.Root, false);
            }
            else
            {
                selectedNode = new TreeProvider().SelectSingleNode(SelectedNodeID, SelectedCulture);
            }

            actionPanel.SelectedNode = selectedNode;
            contentTree.SelectedCulture = SelectedCulture;
            contentTree.SelectedNodeID = SelectedNodeID;
            contentTree.SelectedMode = SelectedMode;

            if (selectedNode == null)
            {
                messageContainer.InnerHtml = "The selected page doesn't exist in the selected culture.";
                messageContainer.Visible = true;
                pnlActions.Visible = false;
                return;
            }

            var url = DocumentURLProvider.GetAbsoluteUrl(selectedNode);
            if (String.IsNullOrEmpty(url))
            {
                actionPanel.AllowIndexSingle = false;
                actionPanel.AllowRefreshSingle = false;
                if (SelectedMode == LayoutMode.Overview)
                {
                    messageContainer.InnerHtml = "The selected page doesn't have a live site URL.";
                    messageContainer.Visible = true;
                }
            }

            if (selectedNode.Children.Count == 0)
            {
                actionPanel.AllowIndexSection = false;
                actionPanel.AllowRefreshSection = false;
            }

            if (SelectedMode == LayoutMode.Overview)
            {
                consoleDetails.Visible = true;
                consoleDetails.StopProcessing = false;
                consoleDetails.SelectedNode = selectedNode;
                consoleDetails.SelectedCulture = SelectedCulture;
            }
            else if (SelectedMode == LayoutMode.Report)
            {
                pnlActions.Visible = false;
                consoleReport.Visible = true;
                consoleReport.StopProcessing = false;
                consoleReport.SelectedNode = selectedNode;
                consoleReport.SelectedCulture = SelectedCulture;
            }
        }


        public enum LayoutMode
        {
            Overview,
            Report
        }
    }
}