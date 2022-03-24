namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#verdict
    /// </summary>
    public class Verdict
    {
        public const string VERDICT_UNSPECIFIED = "VERDICT_UNSPECIFIED";
        public const string PASS = "PASS";
        public const string PARTIAL = "PARTIAL";
        public const string FAIL = "FAIL";
        public const string NEUTRAL = "NEUTRAL";


        public static string GetIcon(string verdict)
        {
            switch (verdict)
            {
                case PASS:
                    return IconSet.Checked("Valid");
                case FAIL:
                    return IconSet.Error("Error");
                case PARTIAL:
                    return IconSet.Warning("Valid with warnings");
                case NEUTRAL:
                    return IconSet.Minus("Unknown");
                case VERDICT_UNSPECIFIED:
                default:
                    return IconSet.Question("Not fetched");
            }
        }


        public static string GetMessage(string verdict)
        {
            switch (verdict)
            {
                case PASS:
                    return "Valid";
                case FAIL:
                    return "Error";
                case PARTIAL:
                    return "Valid with warnings";
                case NEUTRAL:
                    return "Unknown";
                case VERDICT_UNSPECIFIED:
                default:
                    return "Unknown";
            }
        }
    }
}