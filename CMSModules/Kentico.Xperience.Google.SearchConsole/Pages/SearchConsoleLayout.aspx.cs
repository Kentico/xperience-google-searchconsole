using CMS.Base.Web.UI;
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
        public int SelectedMode
        {
            get
            {
                return QueryHelper.GetInteger("SelectedMode", (int)LayoutMode.Overview);
            }
        }


        public int SelectedNodeID
        {
            get
            {
                return QueryHelper.GetInteger("SelectedNodeID", 0);
            }
        }


        public string SelectedCulture
        {
            get
            {
                var culture = QueryHelper.GetString("SelectedCulture", String.Empty);
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

            contentTree.SelectedCulture = SelectedCulture;
            contentTree.SelectedNodeID = SelectedNodeID;
            contentTree.SelectedMode = SelectedMode;

            if (selectedNode == null)
            {
                messageContainer.InnerHtml = "The selected page doesn't exist in the selected culture.";
                messageContainer.Visible = true;
                actionPanel.AllowIndexSingle = false;
                actionPanel.AllowRefreshSingle = false;
                actionPanel.AllowRefreshSection = false;
                return;
            }

            if (SelectedMode == (int)LayoutMode.Overview && selectedNode != null && selectedNode.IsRoot())
            {
                actionPanel.AllowIndexSingle = false;
                actionPanel.AllowRefreshSingle = false;
                return;
            }

            var url = DocumentURLProvider.GetAbsoluteUrl(selectedNode);
            if (String.IsNullOrEmpty(url))
            {
                actionPanel.AllowIndexSingle = false;
                actionPanel.AllowRefreshSingle = false;
            }

            actionPanel.SelectedNode = selectedNode;

            if (SelectedMode == (int)LayoutMode.Overview)
            {
                consoleDetails.Visible = true;
                consoleDetails.StopProcessing = false;
                consoleDetails.SelectedNode = selectedNode;
                consoleDetails.SelectedCulture = SelectedCulture;
            }
            else if (SelectedMode == (int)LayoutMode.Report)
            {
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