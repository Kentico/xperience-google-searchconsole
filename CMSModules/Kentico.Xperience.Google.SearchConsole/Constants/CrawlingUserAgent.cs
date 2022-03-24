namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#crawlinguseragent
    /// </summary>
    public class CrawlingUserAgent
    {
        public const string CRAWLING_USER_AGENT_UNSPECIFIED = "CRAWLING_USER_AGENT_UNSPECIFIED";
        public const string DESKTOP = "DESKTOP";
        public const string MOBILE = "MOBILE";


        public static string GetMessage(string userAgent)
        {
            switch (userAgent)
            {
                case DESKTOP:
                    return "Desktop user agent";
                case MOBILE:
                    return "Mobile user agent";
                case CRAWLING_USER_AGENT_UNSPECIFIED:
                default:
                    return "Unknown user agent";
            }
        }
    }
}