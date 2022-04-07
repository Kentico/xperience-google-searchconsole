namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the overall AMP indexing status of a URL.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#AmpIndexingState"/>.
    /// </remarks>
    public class AmpIndexingState : GoogleApiConstant
    {
        public const string AMP_INDEXING_ALLOWED = "AMP_INDEXING_ALLOWED";
        public const string BLOCKED_DUE_TO_NOINDEX = "BLOCKED_DUE_TO_NOINDEX";
        public const string AMP_INDEXING_STATE_UNSPECIFIED = "AMP_INDEXING_STATE_UNSPECIFIED";
        public const string BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER = "BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER";


        /// <summary>
        /// Initalizes a new instance of the <see cref="AmpIndexingState"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="AmpIndexingState"/> class.</param>
        public AmpIndexingState(string constantValue) : base(constantValue)
        {

        }


        public override string GetIcon()
        {
            switch (constantValue)
            {
                case AMP_INDEXING_ALLOWED:
                    return IconSet.Success("Allowed");
                case BLOCKED_DUE_TO_NOINDEX:
                case BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER:
                    return IconSet.Error("Blocked");
                case AMP_INDEXING_STATE_UNSPECIFIED:
                default:
                    return IconSet.Unknown("Unknown");
            }
        }


        public override string GetMessage()
        {
            switch (constantValue)
            {
                case BLOCKED_DUE_TO_NOINDEX:
                    return "Indexing not allowed, 'noindex' detected";
                case BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER:
                    return "Indexing not allowed, 'unavailable_after' date expired";
                case AMP_INDEXING_ALLOWED:
                    return "Indexing allowed";
                case AMP_INDEXING_STATE_UNSPECIFIED:
                default:
                    return "Unknown";
            }
        }
    }
}