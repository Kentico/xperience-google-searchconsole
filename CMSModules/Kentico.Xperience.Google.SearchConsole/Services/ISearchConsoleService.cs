using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public interface ISearchConsoleService
    {
        OfflineAccessGoogleAuthorizationCodeFlow GoogleAuthorizationCodeFlow
        {
            get;
        }


        TokenResponse GetAccessToken();


        void GetMetadata(IEnumerable<string> urls);


        UserCredential GetUserCredential();
    }
}