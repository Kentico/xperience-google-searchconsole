using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Models;
using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Linq;

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
        private TreeNode selectedNode;


        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            var selectedNodeId = QueryHelper.GetInteger("selectednodeid", 0);
            var selectedCulture = QueryHelper.GetString("selectedculture", String.Empty);
            if (String.IsNullOrEmpty(selectedCulture))
            {
                selectedCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
            }

            if (selectedNodeId == 0)
            {
                selectedNode = new TreeProvider().SelectSingleNode(SiteContext.CurrentSiteName, "/", selectedCulture, true, SystemDocumentTypes.Root, false);
            }
            else
            {
                selectedNode = new TreeProvider().SelectSingleNode(selectedNodeId, selectedCulture);
            }

            InitalizeLayout();
            LoadGridMassActions();
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ctrlMassActions.Visible = !consoleReport.StopProcessing;
        }


        private void InitalizeLayout()
        {
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            if (searchConsoleService.GetUserCredential() == null)
            {
                pnlMain.Visible = false;
                btnAuth.Visible = true;
                return;
            }

            btnAuth.StopProcessing = true;
            contentTree.SelectedCulture = selectedNode.DocumentCulture;
            contentTree.SelectedNodeID = selectedNode.NodeID;
            if (selectedNode == null)
            {
                ShowInformation("The selected page doesn't exist in the selected culture.");
                return;
            }

            consoleDetails.StopProcessing = false;
            consoleDetails.SelectedNode = selectedNode;
            consoleReport.StopProcessing = false;
            consoleReport.SelectedNode = selectedNode;
        }


        private void LoadGridMassActions()
        {
            Func<Func<SearchConsoleMassActionParameters, string>, string, string, CreateUrlDelegate> functionConverter = (generateActionFunction, actionName, title) =>
            {
                return (scope, selectedNodeIDs, parameters) =>
                {
                    var searchConsoleMassActionParameters = new SearchConsoleMassActionParameters
                    {
                        ActionName = actionName,
                        Title = title,
                        NodeIDs = scope == MassActionScopeEnum.AllItems ? selectedNode.Children.Select(i => i.NodeID).ToList() : selectedNodeIDs,
                        Culture = selectedNode.DocumentCulture,
                        ReloadScript = consoleReport.GridReport.GetReloadScript()
                    };
                    return generateActionFunction(searchConsoleMassActionParameters);
                };
            };

            ctrlMassActions.SelectedItemsClientID = consoleReport.GridReport.GetSelectionFieldClientID();
            ctrlMassActions.SelectedItemsResourceString = "Selected pages";
            ctrlMassActions.AllItemsResourceString = "All pages";
            ctrlMassActions.AddMassActions(
                new MassActionItem
                {
                    ActionType = MassActionTypeEnum.OpenModal,
                    CodeName = "Refresh data",
                    CreateUrl = functionConverter(GetMassActionUrl, SearchConsoleConstants.ACTION_REFRESH_DATA, "Refresh data")
                },
                new MassActionItem
                {
                    ActionType = MassActionTypeEnum.OpenModal,
                    CodeName = "Request indexing",
                    CreateUrl = functionConverter(GetMassActionUrl, SearchConsoleConstants.ACTION_REQUEST_INDEXING, "Request indexing")
                }
            );
        }


        /// <summary>
        /// Stores the <paramref name="searchConsoleMassActionParameters"/> in session and generates the absolute URL
        /// to the mass action modal window.
        /// </summary>
        /// <param name="searchConsoleMassActionParameters">Parameters related to the chosen mass action and selected pages.</param>
        /// <returns></returns>
        private string GetMassActionUrl(SearchConsoleMassActionParameters searchConsoleMassActionParameters)
        {
            var paramsIdentifier = Guid.NewGuid().ToString();
            WindowHelper.Add(paramsIdentifier, searchConsoleMassActionParameters);

            var url = URLHelper.ResolveUrl(SearchConsoleConstants.URL_MASSACTION);
            url = URLHelper.AddParameterToUrl(url, "parameters", paramsIdentifier);
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(URLHelper.GetQuery(url)));

            return url;
        }
    }
}