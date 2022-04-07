namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the overall page fetch state of a URL.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#pagefetchstate"/>.
    /// </remarks>
    public class PageFetchState : GoogleApiConstant
    {
        public const string SOFT_404 = "SOFT_404";
        public const string NOT_FOUND = "NOT_FOUND";
        public const string SUCCESSFUL = "SUCCESSFUL";
        public const string INVALID_URL = "INVALID_URL";
        public const string BLOCKED_4XX = "BLOCKED_4XX";
        public const string SERVER_ERROR = "SERVER_ERROR";
        public const string ACCESS_DENIED = "ACCESS_DENIED";
        public const string REDIRECT_ERROR = "REDIRECT_ERROR";
        public const string ACCESS_FORBIDDEN = "ACCESS_FORBIDDEN";
        public const string BLOCKED_ROBOTS_TXT = "BLOCKED_ROBOTS_TXT";
        public const string INTERNAL_CRAWL_ERROR = "INTERNAL_CRAWL_ERROR";
        public const string PAGE_FETCH_STATE_UNSPECIFIED = "PAGE_FETCH_STATE_UNSPECIFIED";


        /// <summary>
        /// Initalizes a new instance of the <see cref="PageFetchState"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="PageFetchState"/> class.</param>
        public PageFetchState(string constantValue) : base(constantValue)
        {

        }


        public override string GetIcon()
        {
            switch (constantValue)
            {
                case SUCCESSFUL:
                    return IconSet.Success("Successful");
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
                    return IconSet.Unknown("Unknown");
            }
        }


        public override string GetMessage()
        {
            switch (constantValue)
            {
                case SUCCESSFUL:
                    return "Successful";
                case ACCESS_FORBIDDEN:
                    return "Access forbidden";
                case ACCESS_DENIED:
                    return "Access denied";
                case SOFT_404:
                    return "Soft 404";
                case BLOCKED_ROBOTS_TXT:
                    return "Blocked by robots.txt";
                case NOT_FOUND:
                    return "Page not found";
                case SERVER_ERROR:
                    return "Server error";
                case REDIRECT_ERROR:
                    return "Redirection error";
                case BLOCKED_4XX:
                    return "Blocked by 4XX error (not 403 or 404)";
                case INTERNAL_CRAWL_ERROR:
                    return "Crawler error";
                case INVALID_URL:
                    return "Invalid URL";
                case PAGE_FETCH_STATE_UNSPECIFIED:
                default:
                    return "Unknown";
            }
        }
    }
}