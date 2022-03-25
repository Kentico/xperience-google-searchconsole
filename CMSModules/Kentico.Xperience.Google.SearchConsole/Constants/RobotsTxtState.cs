namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#robotstxtstate
    /// </summary>
    public class RobotsTxtState
    {
        public const string ROBOTS_TXT_STATE_UNSPECIFIED = "ROBOTS_TXT_STATE_UNSPECIFIED";
        public const string ALLOWED = "ALLOWED";
        public const string DISALLOWED = "DISALLOWED";


        public static string GetIcon(string robotsTxtState)
        {
            switch (robotsTxtState)
            {
                case ALLOWED:
                    return IconSet.Checked("Allowed");
                case DISALLOWED:
                    return IconSet.Error("Disallowed");
                case ROBOTS_TXT_STATE_UNSPECIFIED:
                default:
                    return IconSet.Question("Unknown");
            }
        }


        public static string GetMessage(string robotsTxtState)
        {
            switch (robotsTxtState)
            {
                case DISALLOWED:
                    return "Disallowed";
                case ALLOWED:
                    return "Allowed";
                case ROBOTS_TXT_STATE_UNSPECIFIED:
                default:
                    return "Unknown";
            }
        }
    }
}