namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the overall verdict of an inspection result.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#verdict"/>.
    /// </remarks>
    public class Verdict : GoogleApiConstant
    {
        public const string PASS = "PASS";
        public const string FAIL = "FAIL";
        public const string PARTIAL = "PARTIAL";
        public const string NEUTRAL = "NEUTRAL";
        public const string VERDICT_UNSPECIFIED = "VERDICT_UNSPECIFIED";


        /// <summary>
        /// Initalizes a new instance of the <see cref="Verdict"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="Verdict"/> class.</param>
        public Verdict(string constantValue) : base(constantValue)
        {

        }


        public override string GetIcon()
        {
            switch (constantValue)
            {
                case PASS:
                    return IconSet.Success("Valid");
                case FAIL:
                    return IconSet.Error("Error");
                case PARTIAL:
                    return IconSet.Warning("Valid with warnings");
                case NEUTRAL:
                    return IconSet.Warning("Excluded");
                case VERDICT_UNSPECIFIED:
                default:
                    return IconSet.Unknown("Not fetched");
            }
        }


        public override string GetMessage()
        {
            switch (constantValue)
            {
                case PASS:
                    return "Valid";
                case FAIL:
                    return "Error";
                case PARTIAL:
                    return "Valid with warnings";
                case NEUTRAL:
                    return "Page is not in the index, but not because of an error";
                case VERDICT_UNSPECIFIED:
                default:
                    return "Unknown";
            }
        }
    }
}