using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;

using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class ActionPanel : AbstractUserControl
    {
        private ISearchConsoleService searchConsoleService;


        public TreeNode SelectedNode
        {
            get;
            set;
        }


        public bool AllowRefreshSingle
        {
            get;
            set;
        } = true;


        public bool AllowRefreshSection
        {
            get;
            set;
        } = true;


        public bool AllowIndexSingle
        {
            get;
            set;
        } = true;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            searchConsoleService = Service.Resolve<ISearchConsoleService>();
            btnGetSingleStatus.Enabled = AllowRefreshSingle;
            btnGetSectionStatus.Enabled = AllowRefreshSection;
            btnIndexSingle.Enabled = AllowIndexSingle;
        }


        protected void btnGetSingleStatus_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.GetInspectionResults(new string[] { url }, SelectedNode.DocumentCulture);
            if (result.SuccessfulRequests == 1)
            {
                ShowInformation("Refresh successful.");
            }
            else
            {
                ShowError("Refresh failed. Please check the Event Log for more information.");
            }
        }


        protected void btnGetSectionStatus_Click(object sender, EventArgs e)
        {
            var nodesToUpdate = SelectedNode.AllChildren.ToList();
            nodesToUpdate.Add(SelectedNode);

            var urlsToUpdate = nodesToUpdate.Select(n => DocumentURLProvider.GetAbsoluteUrl(n)).Where(url => !String.IsNullOrEmpty(url));
            if (urlsToUpdate.Count() == 0)
            {
                ShowInformation("The pages in the current section do not have live site URLs.");
                return;
            }

            var result = searchConsoleService.GetInspectionResults(urlsToUpdate, SelectedNode.DocumentCulture);
            if (result.SuccessfulRequests == urlsToUpdate.Count())
            {
                ShowInformation("Refresh successful.");
            }
            else
            {
                ShowError($"{result.FailedRequests}/{urlsToUpdate.Count()} refreshes failed. Please check the Event Log for more information.");
            }
        }


        protected void btnIndexSingle_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.RequestIndexingForPage(url, SelectedNode.DocumentCulture);
            if (result == null)
            {
                ShowError("Indexing request failed. Please check the Event Log for more information.");
                return;
            }

            ShowInformation("Indexing request submitted. Please check Google Search Console or refresh the page status in several days.");
        }
    }
}