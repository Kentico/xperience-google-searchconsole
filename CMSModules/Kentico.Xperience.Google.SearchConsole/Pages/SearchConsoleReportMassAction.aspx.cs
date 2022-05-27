using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Models;
using Kentico.Xperience.Google.SearchConsole.Controls;
using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.UI;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    /// <summary>
    /// A modal dialog which performs a mass action from the <see cref="SearchConsoleReport"/>.
    /// </summary>
    public partial class SearchConsoleReportMassAction : CMSAdministrationPage, ICallbackEventHandler
    {
        private const int SHOWN_RECORDS_NUMBER = 500;
        private IEventLogService eventLogService;
        private ISearchConsoleService searchConsoleService;
        private SearchConsoleMassActionParameters mParameters;
        private string mParametersKey;


        /// <summary>
        /// All errors that occurred during processing.
        /// </summary>
        private string CurrentError
        {
            get
            {
                return ctlAsyncLog.ProcessData.Error;
            }
            set
            {
                ctlAsyncLog.ProcessData.Error = value;
            }
        }


        /// <summary>
        /// The parameters of the chosen mass action.
        /// </summary>
        private SearchConsoleMassActionParameters Parameters
        {
            get
            {
                return mParameters ?? (mParameters = WindowHelper.GetItem(ParametersKey) as SearchConsoleMassActionParameters);
            }
        }


        /// <summary>
        /// The key used to retrieve stored parameters from session.
        /// </summary>
        private string ParametersKey
        {
            get
            {
                return mParametersKey ?? (mParametersKey = QueryHelper.GetString("parameters", String.Empty));
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            eventLogService = Service.Resolve<IEventLogService>();
            searchConsoleService = Service.Resolve<ISearchConsoleService>();

            // Set message placeholder
            if (CurrentMaster != null)
            {
                CurrentMaster.MessagesPlaceHolder = pnlMessagePlaceholder;
            }

            // Register save handler and closing JavaScript 
            var master = CurrentMaster as ICMSModalMasterPage;
            if (master != null)
            {
                master.ShowSaveAndCloseButton();
                master.SetSaveResourceString("Run");
                master.Save += btnRun_OnClick;
                master.SetCloseJavaScript("ReloadAndCallback();");
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (RequestHelper.IsCallback() || !ValidateParameters())
            {
                return;
            }

            ScriptHelper.RegisterWOpenerScript(Page);
            RegisterCallbackScript();

            SetAsyncLogParameters();

            TogglePanels(showContent: true);
            LoadPageView();
        }


        protected void btnRun_OnClick(object sender, EventArgs e)
        {
            TogglePanels(showContent: false);
            ctlAsyncLog.EnsureLog();
            ctlAsyncLog.RunAsync(RunProcess, WindowsIdentity.GetCurrent());
        }


        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            ReturnToListing();
        }


        private void OnFinished(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(CurrentError))
            {
                ShowError("Errors occurred during processing.", description: CurrentError);
            }
            else
            {
                ReturnToListing();
            }
        }


        private void OnCancel(object sender, EventArgs e)
        {
            string cancelled = "The process was cancelled.";
            ctlAsyncLog.AddLog(cancelled);
            LoadPageView();
            ShowWarning(cancelled);
        }


        /// <summary>
        /// Sets visibility of content panel and log panel. Only one can be shown at the time.
        /// </summary>
        private void TogglePanels(bool showContent)
        {
            pnlContent.Visible = showContent;
            pnlLog.Visible = !showContent;
        }


        /// <summary>
        /// Registers a callback script that clears session when dialog is closed.
        /// </summary>
        private void RegisterCallbackScript()
        {
            var callbackEventReference = Page.ClientScript.GetCallbackEventReference(this, String.Empty, "CloseDialog", String.Empty);
            var closeJavaScript = $"function ReloadAndCallback() {{ wopener.{Parameters.ReloadScript}; {callbackEventReference} }}";
            ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "ReloadAndCallback", closeJavaScript, true);
        }


        /// <summary>
        /// Sets parameters of <see cref="AsyncControl"/> dialog.
        /// </summary>
        private void SetAsyncLogParameters()
        {
            ctlAsyncLog.TitleText = Parameters.Title;
            ctlAsyncLog.OnFinished += OnFinished;
            ctlAsyncLog.OnCancel += OnCancel;
        }


        /// <summary>
        /// Ensures that all parameters required for the chosen action are valid, and validates the hash.
        /// </summary>
        private bool ValidateParameters()
        {
            QueryHelper.ValidateHash("hash", settings: new HashSettings(String.Empty) { Redirect = true });

            if (Parameters == null)
            {
                HandleInvalidParameters($"There were no parameters found under the '{ParametersKey}' key.");
                return false;
            }

            if (Parameters.NodeIDs == null ||
                Parameters.NodeIDs.Count() == 0 ||
                String.IsNullOrEmpty(Parameters.Culture) ||
                String.IsNullOrEmpty(Parameters.ReloadScript) ||
                String.IsNullOrEmpty(Parameters.Title))
            {
                HandleInvalidParameters($"One or more parameters are invalid:{Environment.NewLine}{Parameters}");
                return false;
            }

            return true;
        }


        /// <summary>
        /// Shows and logs error.
        /// </summary>
        private void HandleInvalidParameters(string eventDescription)
        {
            eventLogService.LogError(nameof(SearchConsoleReportMassAction), nameof(HandleInvalidParameters), eventDescription);
            RedirectToInformation("The requirements for the chosen action are invalid, please check the Event Log.");
        }


        /// <summary>
        /// Shows page names which will be processed.
        /// </summary>
        private void LoadPageView()
        {
            PageTitle.TitleText = Parameters.Title;
            headAnnouncement.Text = "The action will be performed for the following pages:";
            lblItems.Text = GetDisplayablePageNames();
        }


        /// <summary>
        /// Builds HTML string with the names of the chosen pages.
        /// </summary>
        private string GetDisplayablePageNames()
        {
            var builder = new StringBuilder();
            AppendLimitMessage(builder);

            var names = GetSelectedPages()
                .Take(SHOWN_RECORDS_NUMBER)
                .ToList()
                .Select(page => page.DocumentName);
            foreach (var name in names)
            {
                builder.Append($"<div>&nbsp;{HTMLHelper.HTMLEncode(name)}</div>{Environment.NewLine}");
            }

            // If message is not empty set panel visible
            if (builder.Length > 1)
            {
                pnlItemList.Visible = true;
            }

            return builder.ToString();
        }


        /// <summary>
        /// Eventually appends a message which is shown when more than <see cref="SHOWN_RECORDS_NUMBER"/> pages are about to be processed.
        /// </summary>
        private void AppendLimitMessage(StringBuilder builder)
        {
            if (Parameters.NodeIDs.Count() <= SHOWN_RECORDS_NUMBER)
            {
                return;
            }

            var moreThanMax = String.Format(@"
                <div>
                    <b>{0}</b>
                </div>
                <br />",
                GetString("massdelete.showlimit"));

            builder.AppendFormat(moreThanMax, SHOWN_RECORDS_NUMBER, Parameters.NodeIDs.Count());
        }


        /// <summary>
        /// Performs the chosen mass action within an asynchronous dialog.
        /// </summary>
        private void RunProcess(object parameter)
        {
            var errorLog = new StringBuilder();
            using (var logProgress = new LogContext())
            {
                var selectedPages = GetSelectedPages();
                var selectedUrls = selectedPages.Select(n => DocumentURLProvider.GetAbsoluteUrl(n)).Where(url => !String.IsNullOrEmpty(url));
                if (selectedUrls.Count() > 0)
                {
                    RequestResults result = null;
                    switch (Parameters.ActionName)
                    {
                        case SearchConsoleConstants.ACTION_REFRESH_DATA:
                            result = searchConsoleService.GetInspectionResults(
                                selectedUrls,
                                Parameters.Culture,
                                (url) => AddSuccessLog(logProgress, url),
                                (error) => AddErrorLog(errorLog, error));
                            break;
                        case SearchConsoleConstants.ACTION_REQUEST_INDEXING:
                            result = searchConsoleService.RequestIndexing(
                                selectedUrls,
                                Parameters.Culture,
                                (url) => AddSuccessLog(logProgress, url),
                                (error) => AddErrorLog(errorLog, error));
                            break;
                    }

                    if (result.FailedRequests > 0)
                    {
                        AddErrorLog(errorLog, $"{result.FailedRequests}/{selectedUrls.Count()} requests failed. Please check the Event Log for more information.");
                    }
                }
                else
                {
                    AddErrorLog(errorLog, "The selected pages do not have live site URLs.");
                }
            }
            
            if (errorLog.Length != 0)
            {
                CurrentError = errorLog.ToString();
            }
        }


        /// <summary>
        /// Logs successful action.
        /// </summary>
        /// <param name="logProgress">Log where successful delete will be recorded.</param>
        /// <param name="displayableName">Name of successfully processed item.</param>
        private void AddSuccessLog(LogContext logProgress, string displayableName)
        {
            ctlAsyncLog.AddLog(displayableName);
            string message = $"{displayableName} was processed.";
            logProgress.LogEvent(EventType.INFORMATION, nameof(SearchConsoleReportMassAction), Parameters.ActionName, message, RequestContext.RawURL, CurrentUser.UserID, CurrentUser.UserName,
                0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID, SystemContext.MachineName, RequestContext.URLReferrer, RequestContext.UserAgent, DateTime.Now);
        }


        /// <summary>
        /// Appends the error information to <paramref name="errorLog"/>.
        /// </summary>
        /// <param name="errorLog">Log where errors will be recorded.</param>
        /// <param name="message">Message to add to the log.</param>
        private void AddErrorLog(StringBuilder errorLog, string message)
        {
            ctlAsyncLog.AddLog(message);
            errorLog.Append($"<div>{message}</div>{Environment.NewLine}");
        }


        /// <summary>
        /// Gets the pages to run the process against.
        /// </summary>
        private MultiDocumentQuery GetSelectedPages()
        {
            return DocumentHelper.GetDocuments()
                .OnCurrentSite()
                .Culture(Parameters.Culture)
                .WhereIn(nameof(TreeNode.NodeID), Parameters.NodeIDs.ToList());
        }


        /// <summary>
        /// Redirects back to parent listing.
        /// </summary>
        private void ReturnToListing()
        {
            WindowHelper.Remove(ParametersKey);

            var script = @"wopener." + Parameters.ReloadScript + "; CloseDialog();";
            ScriptHelper.RegisterStartupScript(Page, GetType(), "ReloadGridAndClose", script, addScriptTags: true);
        }


        public void RaiseCallbackEvent(string eventArgument)
        {
            // Raised when Close button in the dialog is clicked, so the parameters can be cleared from session
            WindowHelper.Remove(ParametersKey);
        }


        public string GetCallbackResult()
        {
            // CloseDialog JavaScript method is called to receive the callback results, thus no data needs to be passed to it
            return String.Empty;
        }
    }
}