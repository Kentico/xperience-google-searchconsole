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


        public int NodeID
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


        public string LastCrawl
        {
            get;
            set;
        } = "N/A";


        public string CoverageState
        {
            get;
            set;
        }


        public string VerdictState
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string MobileVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string MobileIssues
        {
            get;
            set;
        }


        public string RichResultsVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        public string RichIssues
        {
            get;
            set;
        }


        public ReportCanonicalUrls CanonicalUrls
        {
            get;
            set;
        }
    }
}