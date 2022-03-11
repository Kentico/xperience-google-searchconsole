using CMS.Base;

namespace Kentico.Xperience.Google.SearchConsole
{
    public static class SearchConsoleConstants
    {
        public const string OAUTH_CALLBACK = "google/oauth/callback";


        public const string TOKEN_CALLBACK = "CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/OAuthAuthorizationFinished.aspx";


        public const string CREDENTIALS_FILENAME = "client_secret.json";


        public const string SETTING_TOKEN = "GoogleSearchConsoleAccessToken";


        public static string fileStorePhysicalPath = $"{SystemContext.WebApplicationPhysicalPath}\\App_Data\\CMSModules\\Kentico.Xperience.Google.SearchConsole";
    }
}