using Kentico.Xperience.Google.SearchConsole;

namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#AmpIndexingState
    /// </summary>
    public class AmpIndexingState
    {
        public const string AMP_INDEXING_STATE_UNSPECIFIED = "AMP_INDEXING_STATE_UNSPECIFIED";
        public const string AMP_INDEXING_ALLOWED = "AMP_INDEXING_ALLOWED";
        public const string BLOCKED_DUE_TO_NOINDEX = "BLOCKED_DUE_TO_NOINDEX";
        public const string BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER = "BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER";


        public static string GetIcon(string ampIndexingState)
        {
            switch (ampIndexingState)
            {
                case AMP_INDEXING_ALLOWED:
                    return IconSet.Checked("Allowed");
                case BLOCKED_DUE_TO_NOINDEX:
                case BLOCKED_DUE_TO_EXPIRED_UNAVAILABLE_AFTER:
                    return IconSet.Error("Blocked");
                case AMP_INDEXING_STATE_UNSPECIFIED:
                default:
                    return IconSet.Question("Unknown");
            }
        }


        public static string GetMessage(string ampIndexingState)
        {
            switch (ampIndexingState)
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