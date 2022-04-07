namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the robots.txt state of a URL.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#robotstxtstate"/>.
    /// </remarks>
    public class RobotsTxtState : GoogleApiConstant
    {
        public const string ALLOWED = "ALLOWED";
        public const string DISALLOWED = "DISALLOWED";
        public const string ROBOTS_TXT_STATE_UNSPECIFIED = "ROBOTS_TXT_STATE_UNSPECIFIED";


        /// <summary>
        /// Initalizes a new instance of the <see cref="RobotsTxtState"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="RobotsTxtState"/> class.</param>
        public RobotsTxtState(string constantValue) : base(constantValue)
        {

        }


        public override string GetIcon()
        {
            switch (constantValue)
            {
                case ALLOWED:
                    return IconSet.Success("Allowed");
                case DISALLOWED:
                    return IconSet.Error("Disallowed");
                case ROBOTS_TXT_STATE_UNSPECIFIED:
                default:
                    return IconSet.Unknown("Unknown");
            }
        }


        public override string GetMessage()
        {
            switch (constantValue)
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