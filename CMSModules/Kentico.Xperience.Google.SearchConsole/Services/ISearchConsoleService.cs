using Google.Apis.Auth.OAuth2;
using Google.Apis.SearchConsole.v1.Data;

using Kentico.Xperience.Google.SearchConsole.Models;

using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public interface ISearchConsoleService
    {
        OfflineAccessGoogleAuthorizationCodeFlow GoogleAuthorizationCodeFlow
        {
            get;
        }


        RequestResults GetInspectionResults(IEnumerable<string> urls, string cultureCode);


        UserCredential GetUserCredential();


        WmxSite GetSite(string xperienceDomain);
    }
}