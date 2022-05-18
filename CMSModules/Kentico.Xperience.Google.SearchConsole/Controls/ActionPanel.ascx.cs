using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;

using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Linq;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    /// <summary>
    /// A control containing the available actions for the Google Search Console application UI.
    /// </summary>
    public partial class ActionPanel : AbstractUserControl
    {
        private ISearchConsoleService searchConsoleService;


        /// <summary>
        /// The node that has been selected from the content tree.
        /// </summary>
        public TreeNode SelectedNode
        {
            get;
            set;
        }


        /// <summary>
        /// If true, the <see cref="SelectedNode"/> can be refreshed.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool AllowRefreshSingle
        {
            get;
            set;
        } = true;


        /// <summary>
        /// If true, the direct children of the <see cref="SelectedNode"/> can be refreshed.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool AllowRefreshSection
        {
            get;
            set;
        } = true;


        /// <summary>
        /// If true, indexing of the <see cref="SelectedNode"/> can be requested.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool AllowIndexSingle
        {
            get;
            set;
        } = true;


        /// <summary>
        /// If true, indexing of the direct children of the <see cref="SelectedNode"/> can be requested.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
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
        }


        /// <summary>
        /// Click handler for the "Refresh single" button. Gets the Google Search Console indexing status for
        /// the <see cref="SelectedNode"/> and stores it in the database.
        /// </summary>
        protected void btnGetSingleStatus_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.GetInspectionResults(new string[] { url }, SelectedNode.DocumentCulture);
            if (result.SuccessfulRequests == 1)
            {
                var redirectUrl = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "refreshed", "1");
                redirectUrl = URLHelper.RemoveParameterFromUrl(redirectUrl, "indexed");
                URLHelper.Redirect(redirectUrl);
            }
            else
            {
                ShowError("Refresh failed. Please check the Event Log for more information.");
            }
        }


        /// <summary>
        /// Click handler for the "Refresh section" button. Gets the Google Search Console indexing statuses for
        /// the <see cref="SelectedNode"/>'s direct children and stores them in the database.
        /// </summary>
        protected void btnGetSectionStatus_Click(object sender, EventArgs e)
        {
            var nodesToUpdate = SelectedNode.Children.ToList();
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
                var redirectUrl = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "refreshed", "1");
                redirectUrl = URLHelper.RemoveParameterFromUrl(redirectUrl, "indexed");
                URLHelper.Redirect(redirectUrl);
            }
        }


        /// <summary>
        /// Click handler for the "Index single" button. Requests that the <see cref="SelectedNode"/> be indexed by Google,
        /// and stores the request time in the database.
        /// </summary>
        protected void btnIndexSingle_Click(object sender, EventArgs e)
        {
            var url = DocumentURLProvider.GetAbsoluteUrl(SelectedNode);
            var result = searchConsoleService.RequestIndexing(new string[] { url }, SelectedNode.DocumentCulture);
            if (result.SuccessfulRequests == 1)
            {
                var redirectUrl = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "indexed", "1");
                redirectUrl = URLHelper.RemoveParameterFromUrl(redirectUrl, "refreshed");
                URLHelper.Redirect(redirectUrl);
            }
            else
            {
                ShowError("Indexing request failed. Please check the Event Log for more information.");
            }   
        }


        /// <summary>
        /// Click handler for the "Index section" button. Requests that the direct children of the <see cref="SelectedNode"/>
        /// be indexed by Google, and stores the request times in the database.
        /// </summary>
        protected void btnIndexSection_Click(object sender, EventArgs e)
        {
            var nodesToUpdate = SelectedNode.Children.ToList();
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
                var redirectUrl = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "indexed", "1");
                redirectUrl = URLHelper.RemoveParameterFromUrl(redirectUrl, "refreshed");
                URLHelper.Redirect(redirectUrl);
            }
        }
    }
}