namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#pagefetchstate
    /// </summary>
    public class PageFetchState
    {
        public const string PAGE_FETCH_STATE_UNSPECIFIED = "PAGE_FETCH_STATE_UNSPECIFIED";
        public const string SUCCESSFUL = "SUCCESSFUL";
        public const string SOFT_404 = "SOFT_404";
        public const string BLOCKED_ROBOTS_TXT = "BLOCKED_ROBOTS_TXT";
        public const string NOT_FOUND = "NOT_FOUND";
        public const string ACCESS_DENIED = "ACCESS_DENIED";
        public const string SERVER_ERROR = "SERVER_ERROR";
        public const string REDIRECT_ERROR = "REDIRECT_ERROR";
        public const string ACCESS_FORBIDDEN = "ACCESS_FORBIDDEN";
        public const string BLOCKED_4XX = "BLOCKED_4XX";
        public const string INTERNAL_CRAWL_ERROR = "INTERNAL_CRAWL_ERROR";
        public const string INVALID_URL = "INVALID_URL";


        public static string GetIcon(string pageFetchState)
        {
            switch (pageFetchState)
            {
                case SUCCESSFUL:
                    return IconSet.Checked("Successful");
                case SOFT_404:
                case BLOCKED_ROBOTS_TXT:
                case NOT_FOUND:
                case ACCESS_DENIED:
                case SERVER_ERROR:
                case REDIRECT_ERROR:
                case ACCESS_FORBIDDEN:
                case BLOCKED_4XX:
                case INTERNAL_CRAWL_ERROR:
                case INVALID_URL:
                    return IconSet.Error("Error");
                case PAGE_FETCH_STATE_UNSPECIFIED:
                default:
                    return IconSet.Question("Unknown");
            }
        }
    }
}