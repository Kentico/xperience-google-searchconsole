using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

using System;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    public partial class SearchConsoleLayout : CMSPage
    {
        public int SelectedMode
        {
            get
            {
                return QueryHelper.GetInteger(nameof(SelectedMode), (int)LayoutMode.Overview);
            }
        }


        public int SelectedNodeID
        {
            get
            {
                var nodeId = QueryHelper.GetInteger(nameof(SelectedNodeID), 0);
                if (nodeId == 0)
                {
                    return new TreeProvider().SelectSingleNode(SiteContext.CurrentSiteName, "/", SelectedCulture, true, SystemDocumentTypes.Root, false).NodeID;
                }

                return nodeId;
            }
        }


        public string SelectedCulture
        {
            get
            {
                var culture = QueryHelper.GetString(nameof(SelectedCulture), String.Empty);
                if (String.IsNullOrEmpty(culture))
                {
                    culture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
                }

                return culture;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (SelectedMode == (int)LayoutMode.Overview)
            {
                uipDetails.ControlPath = "~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsolePageDetails.ascx";
            }
            else if (SelectedMode == (int)LayoutMode.Report)
            {
                uipDetails.ControlPath = "~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsoleReport.ascx";
            }
        }


        public enum LayoutMode
        {
            Overview,
            Report
        }
    }
}