namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#IndexingState
    /// </summary>
    public class IndexingState
    {
        public const string INDEXING_STATE_UNSPECIFIED = "INDEXING_STATE_UNSPECIFIED";
        public const string INDEXING_ALLOWED = "INDEXING_ALLOWED";
        public const string BLOCKED_BY_META_TAG = "BLOCKED_BY_META_TAG";
        public const string BLOCKED_BY_HTTP_HEADER = "BLOCKED_BY_HTTP_HEADER";
        public const string BLOCKED_BY_ROBOTS_TXT = "BLOCKED_BY_ROBOTS_TXT";


        public static string GetIcon(string indexingState)
        {
            switch (indexingState)
            {
                case INDEXING_ALLOWED:
                    return IconSet.Checked("Allowed");
                case BLOCKED_BY_META_TAG:
                case BLOCKED_BY_HTTP_HEADER:
                case BLOCKED_BY_ROBOTS_TXT:
                    return IconSet.Error("Blocked");
                case INDEXING_STATE_UNSPECIFIED:
                default:
                    return IconSet.Question("Unknown");
            }
        }


        public static string GetMessage(string indexingState)
        {
            switch (indexingState)
            {
                case BLOCKED_BY_META_TAG:
                    return "Indexing not allowed, 'noindex' detected in 'robots' meta tag";
                case BLOCKED_BY_HTTP_HEADER:
                    return "Indexing not allowed, 'noindex' detected in 'X-Robots-Tag' http header";
                case INDEXING_ALLOWED:
                    return "Indexing allowed";
                case INDEXING_STATE_UNSPECIFIED:
                default:
                    return "Unknown";
            }
        }
    }
}