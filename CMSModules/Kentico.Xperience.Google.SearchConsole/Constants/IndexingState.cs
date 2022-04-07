namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the overall indexing state of a URL.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#IndexingState"/>.
    /// </remarks>
    public class IndexingState : GoogleApiConstant
    {
        public const string INDEXING_ALLOWED = "INDEXING_ALLOWED";
        public const string BLOCKED_BY_META_TAG = "BLOCKED_BY_META_TAG";
        public const string BLOCKED_BY_ROBOTS_TXT = "BLOCKED_BY_ROBOTS_TXT";
        public const string BLOCKED_BY_HTTP_HEADER = "BLOCKED_BY_HTTP_HEADER";
        public const string INDEXING_STATE_UNSPECIFIED = "INDEXING_STATE_UNSPECIFIED";


        /// <summary>
        /// Initalizes a new instance of the <see cref="IndexingState"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="IndexingState"/> class.</param>
        public IndexingState(string constantValue) : base(constantValue)
        {

        }


        public override string GetIcon()
        {
            switch (constantValue)
            {
                case INDEXING_ALLOWED:
                    return IconSet.Success("Allowed");
                case BLOCKED_BY_META_TAG:
                case BLOCKED_BY_HTTP_HEADER:
                case BLOCKED_BY_ROBOTS_TXT:
                    return IconSet.Error("Blocked");
                case INDEXING_STATE_UNSPECIFIED:
                default:
                    return IconSet.Unknown("Unknown");
            }
        }


        public override string GetMessage()
        {
            switch (constantValue)
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