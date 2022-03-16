using CMS.Base;

namespace Kentico.Xperience.Google.SearchConsole.Constants
{
    public static class SearchConsoleConstants
    {
        public const string DEFAULT_USER = "searchconsoleuser";


        public const string APPSETTING_APPLICATIONNAME = "CMSApplicationName";


        public const string SESSION_SELECTEDCULTURE = "searchconsole_treeculture";


        public const string OAUTH_CALLBACK = "Admin/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/OAuthCallback.aspx";


        public const string TOKEN_CALLBACK = "CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/OAuthAuthorizationFinished.aspx";


        public const string CREDENTIALS_FILENAME = "client_secret.json";


        public const string SETTING_TOKEN = "GoogleSearchConsoleAccessToken";


        public static string fileStorePhysicalPath = $"{SystemContext.WebApplicationPhysicalPath}\\App_Data\\CMSModules\\Kentico.Xperience.Google.SearchConsole";
    }
}