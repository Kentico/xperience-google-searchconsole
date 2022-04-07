using System;

namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    /// <summary>
    /// Contains constants from the Google Search Console API representing the user agent used for crawling a URL.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developers.google.com/webmaster-tools/v1/urlInspection.index/UrlInspectionResult#crawlinguseragent"/>.
    /// </remarks>
    public class CrawlingUserAgent : GoogleApiConstant
    {
        public const string MOBILE = "MOBILE";
        public const string DESKTOP = "DESKTOP";
        public const string CRAWLING_USER_AGENT_UNSPECIFIED = "CRAWLING_USER_AGENT_UNSPECIFIED";


        /// <summary>
        /// Initalizes a new instance of the <see cref="CrawlingUserAgent"/> class.
        /// </summary>
        /// <param name="constantValue">A constant value from the <see cref="CrawlingUserAgent"/> class.</param>
        public CrawlingUserAgent(string constantValue) : base(constantValue)
        {

        }

        public override string GetIcon()
        {
            throw new NotImplementedException();
        }


        public override string GetMessage()
        {
            switch (constantValue)
            {
                case DESKTOP:
                    return "Desktop user agent";
                case MOBILE:
                    return "Mobile user agent";
                case CRAWLING_USER_AGENT_UNSPECIFIED:
                default:
                    return "Unknown user agent";
            }
        }
    }
}