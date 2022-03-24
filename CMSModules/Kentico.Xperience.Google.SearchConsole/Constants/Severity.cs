namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#Severity
    /// </summary>
    public class Severity
    {
        public const string SEVERITY_UNSPECIFIED = "SEVERITY_UNSPECIFIED";
        public const string WARNING = "WARNING";
        public const string ERROR = "ERROR";


        public static string GetIcon(string severity)
        {
            switch (severity)
            {
                case WARNING:
                    return IconSet.Warning("Warning");
                case ERROR:
                    return IconSet.Error("Error");
                case SEVERITY_UNSPECIFIED:
                default:
                    return IconSet.Question("Not fetched");
            }
        }
    }
}