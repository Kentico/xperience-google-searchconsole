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
    }
}