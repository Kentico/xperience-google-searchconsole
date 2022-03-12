using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;

using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class SearchConsoleActions : AbstractUserControl
    {
        private string cultureCode;
        private int selectedNodeId = 0;
        private IEnumerable<int> sectionNodeIds = Enumerable.Empty<int>();
        private IEventLogService eventLogService;
        private ISearchConsoleService searchConsoleService;


        private ModeEnum Mode
        {
            get
            {
                var selectedMode = ValidationHelper.GetInteger(drpMode.SelectedValue, 0);
                return (ModeEnum)selectedMode;
            }
        }


        private ActionEnum Action
        {
            get
            {
                var selectedAction = ValidationHelper.GetInteger(drpAction.SelectedValue, 0);
                return (ActionEnum)selectedAction;
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            eventLogService = Service.Resolve<IEventLogService>();
            searchConsoleService = Service.Resolve<ISearchConsoleService>();

            InitDropDowns();
        }


        public void InitializeNodeData(int selectedNodeId, IEnumerable<int> sectionNodeIds, string cultureCode)
        {
            this.selectedNodeId = selectedNodeId;
            this.sectionNodeIds = sectionNodeIds;
            this.cultureCode = cultureCode;
        }


        protected void Submit(object sender, EventArgs e)
        {
            if ((Mode == ModeEnum.SELECTED_NODE && selectedNodeId == 0) ||
                (Mode == ModeEnum.SECTION && sectionNodeIds.Count() == 0))
            {
                eventLogService.LogError(nameof(SearchConsoleActions), nameof(Submit), "A node or section was requested, but the node IDs were not set.");
                ShowError("Unable to submit request, please check the Event Log.");
                return;
            }

            if (String.IsNullOrEmpty(cultureCode))
            {
                eventLogService.LogError(nameof(SearchConsoleActions), nameof(Submit), "The culture of the selected nodes is not set.");
                ShowError("Unable to submit request, please check the Event Log.");
                return;
            }

            var nodesToUpdate = new List<CMS.DocumentEngine.TreeNode>();
            if (Mode == ModeEnum.SELECTED_NODE)
            {
                var node = new TreeProvider().SelectSingleNode(selectedNodeId, cultureCode);
                nodesToUpdate.Add(node);
            }
            else if (Mode == ModeEnum.SECTION)
            {
                var nodes = new TreeProvider().SelectNodes()
                    .Culture(cultureCode)
                    .WhereIn(nameof(CMS.DocumentEngine.TreeNode.NodeID), sectionNodeIds.ToList())
                    .TypedResult;
                nodesToUpdate.AddRange(nodes);
            }
            else if (Mode == ModeEnum.TREE)
            {
                var allPages = new TreeProvider().SelectNodes().Culture(cultureCode).TypedResult;
                nodesToUpdate.AddRange(allPages);
            }

            var urlsToUpdate = nodesToUpdate.Select(n => DocumentURLProvider.GetAbsoluteUrl(n)).Where(url => !String.IsNullOrEmpty(url));
            if (urlsToUpdate.Count() == 0)
            {
                ShowInformation("The selected node(s) do not have live site URLs.");
                return;
            }

            if (Action == ActionEnum.GET_METADATA)
            {
                searchConsoleService.GetMetadata(urlsToUpdate);
            }
        }


        private void InitDropDowns()
        {
            drpMode.Items.AddRange(new ListItem[] {
                new ListItem("Selected node", ModeEnum.SELECTED_NODE.ToString()),
                new ListItem("Current section", ModeEnum.SECTION.ToString()),
                new ListItem("Content tree", ModeEnum.TREE.ToString())
            });
            drpAction.Items.AddRange(new ListItem[] {
                new ListItem("Refresh status", ActionEnum.GET_METADATA.ToString()),
                new ListItem("Request update", ActionEnum.REQUEST_UPDATE.ToString())
            });
        }


        private enum ActionEnum
        {
            GET_METADATA,
            REQUEST_UPDATE
        }


        private enum ModeEnum
        {
            SELECTED_NODE,
            SECTION,
            TREE
        }
    }
}