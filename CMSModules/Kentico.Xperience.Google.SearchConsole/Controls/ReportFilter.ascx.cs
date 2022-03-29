using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class ReportFilter : AbstractUserControl, IFilterControl
    {
        public Control FilteredControl
        {
            get;
            set;
        }


        public object Value
        {
            get
            {
                return drpShowErrors.SelectedValue;
            }
            set
            {
                drpShowErrors.SelectedValue = value as string;
            }
        }


        public string WhereCondition
        {
            get;
            set;
        }


        public event ActionEventHandler OnFilterChanged;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            drpShowErrors.Items.AddRange(new ListItem[] {
                new ListItem("(all)", FilterOptions.ALL.ToString()),
                new ListItem("Coverage issues", FilterOptions.COVERAGE.ToString()),
                new ListItem("Indexing issues", FilterOptions.INDEX.ToString()),
                new ListItem("Crawling issues", FilterOptions.CRAWL.ToString()),
                new ListItem("Page fetch issues", FilterOptions.FETCH.ToString()),
                new ListItem("Mobile issues", FilterOptions.MOBILE.ToString()),
                new ListItem("Rich results issues", FilterOptions.RICH.ToString()),
                new ListItem("AMP issues", FilterOptions.AMP.ToString()),
                new ListItem("Canonical URL mismatches", FilterOptions.CANONICAL.ToString())
            });
        }


        public List<ReportItem> Apply(List<ReportItem> dataToFilter)
        {
            var selectedFilter = ValidationHelper.GetString(drpShowErrors.SelectedValue, String.Empty);
            if (String.IsNullOrEmpty(selectedFilter) || selectedFilter == FilterOptions.ALL.ToString())
            {
                return dataToFilter;
            }

            FilterOptions filterOption;
            if (!Enum.TryParse(selectedFilter, out filterOption))
            {
                return dataToFilter;
            }

            switch (filterOption)
            {
                case FilterOptions.COVERAGE:
                    return dataToFilter.Where(item => item.CoverageVerdict != Verdict.PASS && item.CoverageVerdict != Verdict.VERDICT_UNSPECIFIED).ToList();
                case FilterOptions.INDEX:
                    return dataToFilter.Where(item => item.IndexState != IndexingState.INDEXING_ALLOWED && item.IndexState != IndexingState.INDEXING_STATE_UNSPECIFIED).ToList();
                case FilterOptions.CRAWL:
                    return dataToFilter.Where(item => item.CrawlState != RobotsTxtState.ALLOWED && item.CrawlState != RobotsTxtState.ROBOTS_TXT_STATE_UNSPECIFIED).ToList();
                case FilterOptions.FETCH:
                    return dataToFilter.Where(item => item.FetchState != PageFetchState.SUCCESSFUL && item.FetchState != PageFetchState.PAGE_FETCH_STATE_UNSPECIFIED).ToList();
                case FilterOptions.MOBILE:
                    return dataToFilter.Where(item => item.MobileVerdict != Verdict.PASS && item.MobileVerdict != Verdict.VERDICT_UNSPECIFIED).ToList();
                case FilterOptions.RICH:
                    return dataToFilter.Where(item => item.RichResultsVerdict != Verdict.PASS && item.RichResultsVerdict != Verdict.VERDICT_UNSPECIFIED).ToList();
                case FilterOptions.AMP:
                    return dataToFilter.Where(item => item.AmpVerdict != AmpIndexingState.AMP_INDEXING_ALLOWED && item.AmpVerdict != AmpIndexingState.AMP_INDEXING_STATE_UNSPECIFIED).ToList();
                case FilterOptions.CANONICAL:
                    return dataToFilter.Where(item => item.CanonicalUrls != null &&
                        !String.IsNullOrEmpty(item.CanonicalUrls.UserUrl) &&
                        !String.IsNullOrEmpty(item.CanonicalUrls.GoogleUrl) &&
                        item.CanonicalUrls.GoogleUrl != item.CanonicalUrls.UserUrl
                    ).ToList();
                default:
                    return dataToFilter;
            }
        }


        public void ResetFilter()
        {
            drpShowErrors.SelectedIndex = 0;
        }


        public void RestoreFilterState(FilterState state)
        {

        }


        public void StoreFilterState(FilterState state)
        {

        }


        protected enum FilterOptions
        {
            ALL,
            COVERAGE,
            INDEX,
            CRAWL,
            FETCH,
            MOBILE,
            RICH,
            AMP,
            CANONICAL
        }
    }
}