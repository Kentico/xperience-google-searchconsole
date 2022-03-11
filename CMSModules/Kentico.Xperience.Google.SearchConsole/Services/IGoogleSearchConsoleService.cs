using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public interface IGoogleSearchConsoleService
    {
        GoogleAuthorizationCodeFlow GoogleAuthorizationCodeFlow
        {
            get;
        }


        TokenResponse GetAccessToken();
    }
}