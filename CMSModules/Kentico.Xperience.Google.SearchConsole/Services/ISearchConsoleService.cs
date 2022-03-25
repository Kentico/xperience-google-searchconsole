using Google.Apis.Auth.OAuth2;
using Google.Apis.Indexing.v3.Data;
using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Models;
using Kentico.Xperience.Google.SearchConsole.Constants;

using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public interface ISearchConsoleService
    {
        OfflineAccessGoogleAuthorizationCodeFlow GoogleAuthorizationCodeFlow
        {
            get;
        }


        /// <summary>
        /// Gets the absolute URL used in Google's OAuth authentication redirect, which should match the
        /// value saved in the Google Cloud Console and client secrets file. Finds a site or site alias
        /// that starts with the current request domain to locate the appropriate path of the administration,
        /// e.g. "mysite.com" or "mysite.com/Admin."
        /// </summary>
        /// <returns>The full administrative URL with protocol, appended with <see cref="SearchConsoleConstants.OAUTH_CALLBACK"/>.</returns>
        string GetUrlForCallback();


        RequestResults GetInspectionResults(IEnumerable<string> urls, string cultureCode);


        UserCredential GetUserCredential();


        WmxSite GetSite(string xperienceDomain);


        PublishUrlNotificationResponse RequestIndexingForPage(string url, string cultureCode);
    }
}