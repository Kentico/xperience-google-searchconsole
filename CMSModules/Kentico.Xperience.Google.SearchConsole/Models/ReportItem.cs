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


        public string Coverage
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public int ReferringUrlCount
        {
            get;
            set;
        }


        public string Sitemaps
        {
            get;
            set;
        } = "N/A";


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


        public string CrawlAllowed
        {
            get;
            set;
        } = "N/A";


        public string PageFetch
        {
            get;
            set;
        } = "N/A";


        public string IndexingAllowed
        {
            get;
            set;
        } = "N/A";


        public string CanonicalMatch
        {
            get;
            set;
        } = "N/A";
    }
}