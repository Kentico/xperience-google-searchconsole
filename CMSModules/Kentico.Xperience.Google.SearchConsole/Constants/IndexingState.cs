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
    }
}