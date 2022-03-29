using Kentico.Xperience.Google.SearchConsole.Constants;

namespace Kentico.Xperience.Google.SearchConsole.Models
{
    public class ReportItem
    {
        public int InspectionStatusID
        {
            get;
            set;
        }


        public string DocumentName
        {
            get;
            set;
        }


        public string Url
        {
            get;
            set;
        }


        public string LastRefresh
        {
            get;
            set;
        } = "N/A";


        public string CoverageVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string MobileVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string RichResultsVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string AmpVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string IndexState
        {
            get;
            set;
        } = IndexingState.INDEXING_STATE_UNSPECIFIED;


        public string FetchState
        {
            get;
            set;
        } = PageFetchState.PAGE_FETCH_STATE_UNSPECIFIED;


        public string CrawlState
        {
            get;
            set;
        } = RobotsTxtState.ROBOTS_TXT_STATE_UNSPECIFIED;


        public int ReferringUrlCount
        {
            get;
            set;
        }


        public string LastCrawl
        {
            get;
            set;
        } = "N/A";


        public string CrawledAs
        {
            get;
            set;
        } = "N/A";


        public ReportCanonicalUrls CanonicalUrls
        {
            get;
            set;
        }
    }
}