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


        public bool AllowIndexSection
        {
            get;
            set;
        } = true;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            searchConsoleService = Service.Resolve<ISearchConsoleService>();

            if (SelectedNode == null)
            {
                btnGetSingleStatus.Enabled = false;
                btnGetSectionStatus.Enabled = false;
                btnIndexSingle.Enabled = false;
                btnIndexSection.Enabled = false;
                return;
            }

            btnGetSingleStatus.Enabled = AllowRefreshSingle;
            btnGetSectionStatus.Enabled = AllowRefreshSection;
            btnIndexSingle.Enabled = AllowIndexSingle;
            btnIndexSection.Enabled = AllowIndexSection;

            var singleAlias = $"/{SelectedNode.NodeAlias}";
            var sectionAlias = $"/{SelectedNode.NodeAlias}/%";
            if (SelectedNode.IsRoot())
            {
                singleAlias = "page";
                sectionAlias = "/%";
            }

            btnGetSingleStatus.Text = $"Refresh {singleAlias}";
            btnGetSectionStatus.Text = $"Refresh {sectionAlias}";
            btnIndexSingle.Text = $"Index {singleAlias}";
            btnIndexSection.Text = $"Index {sectionAlias}";
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
            if (result.FailedRequests > 0)
            {
                ShowError($"{result.FailedRequests}/{urlsToUpdate.Count()} refreshes failed. Please check the Event Log for more information.");
            }
            else
            {
                ShowInformation("Refresh successful.");
            }
        }


        protected void btnIndexSingle_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.RequestIndexing(new string[] { url }, SelectedNode.DocumentCulture);
            if (result.SuccessfulRequests == 1)
            {
                ShowInformation("Indexing request submitted. Please check Google Search Console or refresh the page status in several days.");
            }
            else
            {
                ShowError("Indexing request failed. Please check the Event Log for more information.");
            }   
        }


        protected void btnIndexSection_Click(object sender, EventArgs e)
        {
            var nodesToUpdate = SelectedNode.AllChildren.ToList();
            nodesToUpdate.Add(SelectedNode);

            var urlsToUpdate = nodesToUpdate.Select(n => DocumentURLProvider.GetAbsoluteUrl(n)).Where(url => !String.IsNullOrEmpty(url));
            if (urlsToUpdate.Count() == 0)
            {
                ShowInformation("The pages in the current section do not have live site URLs.");
                return;
            }

            var result = searchConsoleService.RequestIndexing(urlsToUpdate, SelectedNode.DocumentCulture);
            if (result.FailedRequests > 0)
            {
                ShowError($"{result.FailedRequests}/{urlsToUpdate.Count()} indexing request failed. Please check the Event Log for more information.");
            }
            else
            {
                ShowInformation("Indexing requests submitted. Please check Google Search Console or refresh the page statuses in several days.");
            }
        }
    }
}