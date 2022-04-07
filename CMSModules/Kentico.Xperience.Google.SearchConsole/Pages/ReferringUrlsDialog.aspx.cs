using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;

using Google.Apis.SearchConsole.v1.Data;

using Newtonsoft.Json;

using System;
using System.Text;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    /// <summary>
    /// A dialog page that displays the referring URLs found in a <see cref="UrlInspectionStatusInfo.LastInspectionResult"/>,
    /// which is obtained using an ID in the query string.
    /// </summary>
    public partial class ReferringUrlsDialog : CMSModalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string title;
            var inspectionStatusID = QueryHelper.GetInteger("inspectionStatusID", 0);
            var inspectionStatusProvider = Service.Resolve<IUrlInspectionStatusInfoProvider>();
            var inspectionStatus = inspectionStatusProvider.Get(inspectionStatusID);
            if (inspectionStatus == null)
            {
                title = "Error";
                PageTitle.TitleText = title;
                Page.Title = title;

                ltlUrls.Text = "Error loading the Google Search Console data from the database.";

                return;
            }

            title = "Referring URLs";
            PageTitle.TitleText = title;
            Page.Title = title;

            var sb = new StringBuilder();
            var inspectUrlIndexResponse = JsonConvert.DeserializeObject<InspectUrlIndexResponse>(inspectionStatus.LastInspectionResult);
            foreach (var url in inspectUrlIndexResponse.InspectionResult.IndexStatusResult.ReferringUrls)
            {
                sb.Append(url).Append("<br/>");
            }

            ltlUrls.Text = sb.ToString();
        }
    }
}