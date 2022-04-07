using CMS.DocumentEngine;

using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Constants;

using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Models
{
    /// <summary>
    /// Represents the data of a page's <see cref="UrlInspectionStatusInfo"/> suitable for displaying in a UniGrid.
    /// </summary>
    public class ReportItem
    {
        /// <summary>
        /// The page's <see cref="UrlInspectionStatusInfo.PageIndexStatusID"/>.
        /// </summary>
        public int PageIndexStatusID
        {
            get;
            set;
        }


        /// <summary>
        /// The page's <see cref="TreeNode.NodeID"/>.
        /// </summary>
        public int NodeID
        {
            get;
            set;
        }


        /// <summary>
        /// The page's <see cref="TreeNode.DocumentName"/>.
        /// </summary>
        public string DocumentName
        {
            get;
            set;
        }


        /// <summary>
        /// The page's live site URL.
        /// </summary>
        public string Url
        {
            get;
            set;
        }


        /// <summary>
        /// The page's <see cref="UrlInspectionStatusInfo.InspectionResultRequestedOn"/>.
        /// </summary>
        public string LastRefresh
        {
            get;
            set;
        } = "N/A";


        /// <summary>
        /// The page's <see cref="IndexStatusInspectionResult.LastCrawlTime"/>.
        /// </summary>
        public string LastCrawl
        {
            get;
            set;
        } = "N/A";


        /// <summary>
        /// The page's <see cref="IndexStatusInspectionResult.CoverageState"/>.
        /// </summary>
        public string CoverageState
        {
            get;
            set;
        }


        /// <summary>
        /// The page's <see cref="IndexStatusInspectionResult.Verdict"/>.
        /// </summary>
        public string VerdictState
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        /// <summary>
        /// The page's <see cref="MobileUsabilityInspectionResult.Verdict"/>.
        /// </summary>
        public string MobileVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        /// <summary>
        /// The page's <see cref="MobileUsabilityInspectionResult.Issues"/>.
        /// </summary>
        public IEnumerable<MobileUsabilityIssue> MobileIssues
        {
            get;
            set;
        }


        /// <summary>
        /// The page's <see cref="RichResultsInspectionResult.Verdict"/>.
        /// </summary>
        public string RichResultsVerdict
        {
            get;
            set;
        } = Verdict.VERDICT_UNSPECIFIED;


        /// <summary>
        /// The page's <see cref="Item.Issues"/> from the <see cref="RichResultsInspectionResult.DetectedItems"/>.
        /// </summary>
        public IEnumerable<RichResultsIssue> RichIssues
        {
            get;
            set;
        }
    }
}