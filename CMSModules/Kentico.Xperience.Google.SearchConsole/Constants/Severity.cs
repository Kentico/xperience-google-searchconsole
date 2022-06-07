using System;

namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the severity of an issue.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#Severity"/>.
    /// </remarks>
    public class Severity : GoogleApiConstant
    {
        public const string ERROR = "ERROR";
        public const string WARNING = "WARNING";
        public const string SEVERITY_UNSPECIFIED = "SEVERITY_UNSPECIFIED";


        /// <summary>
        /// Initalizes a new instance of the <see cref="Severity"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="Severity"/> class.</param>
        public Severity(string constantValue) : base(constantValue)
        {

        }


        public override string GetIcon()
        {
            switch (constantValue)
            {
                case WARNING:
                    return IconSet.Warning("Warning");
                case ERROR:
                    return IconSet.Error("Error");
                case SEVERITY_UNSPECIFIED:
                default:
                    return IconSet.Error("Unknown");
            }
        }


        public override string GetMessage()
        {
            throw new NotImplementedException();
        }
    }
}