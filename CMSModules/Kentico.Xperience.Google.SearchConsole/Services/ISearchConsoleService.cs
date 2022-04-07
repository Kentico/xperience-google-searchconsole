using Google.Apis.Auth.OAuth2;
using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Models;
using Kentico.Xperience.Google.SearchConsole.Constants;

using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Services
{
    /// <summary>
    /// Contains methods for authorizing against Google OAuth and making Google API requests.
    /// </summary>
    public interface ISearchConsoleService
    {
        /// <summary>
        /// An authorization process that can be used to authenticate with Google OAuth or obtain the
        /// existing OAuth token. Uses "offline" access to automatically refresh the OAuth token without
        /// user interaction.
        /// </summary>
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


        /// <summary>
        /// Requests the Google Search Console <see cref="UrlInspectionResult"/> for Xperience live site URLs
        /// and stores them in the Xperience database as <see cref="UrlInspectionStatusInfo"/> objects.
        /// </summary>
        /// <param name="urls">The live site URLs to retrieve the indexing status for.</param>
        /// <param name="cultureCode">The culture code of the <paramref name="urls"/>.</param>
        /// <returns>A <see cref="RequestResults"/> object containing the number of successful/failed requests,
        /// and the errors that were encountered.</returns>
        RequestResults GetInspectionResults(IEnumerable<string> urls, string cultureCode);


        /// <summary>
        /// Gets the credentials (including token) from the <see cref="GoogleAuthorizationCodeFlow"/>, or
        /// null if not authenticated yet. Refreshes the OAuth token if it has expired.
        /// </summary>
        UserCredential GetUserCredential();


        /// <summary>
        /// Gets a property which has been registered in Google Search Console by checking whether the property
        /// contains the provided <paramref name="xperienceDomain"/>.
        /// </summary>
        /// <param name="xperienceDomain">The domain of the Xperience live site used to find a matching property
        /// in Google Search Console.</param>
        /// <returns>A registered Google Search Console property, or null if not found.</returns>
        WmxSite GetSite(string xperienceDomain);


        /// <summary>
        /// Submits a request for Google to index the provided <paramref name="urls"/>. Stores the request time in
        /// the Xperience database in the corresponding <see cref="UrlInspectionStatusInfo"/> objects.
        /// </summary>
        /// <param name="urls">The live site URLs to index.</param>
        /// <param name="cultureCode">The culture code of the <paramref name="urls"/>.</param>
        /// <returns>A <see cref="RequestResults"/> object containing the number of successful/failed requests,
        /// and the errors that were encountered.</returns>
        RequestResults RequestIndexing(IEnumerable<string> urls, string cultureCode);
    }
}