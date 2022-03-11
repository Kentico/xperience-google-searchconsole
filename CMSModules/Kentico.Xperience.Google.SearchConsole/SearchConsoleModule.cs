using CMS;
using CMS.DataEngine;

using Kentico.Xperience.Google.SearchConsole;

using System.Web.Http;

[assembly: RegisterModule(typeof(SearchConsoleModule))]
namespace Kentico.Xperience.Google.SearchConsole
{
    public class SearchConsoleModule : Module
    {
        public SearchConsoleModule() : base(nameof(SearchConsoleModule))
        {

        }


        protected override void OnInit()
        {
            base.OnInit();

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                "googlesearchconsole",
                SearchConsoleConstants.OAUTH_CALLBACK,
                defaults: new { controller = "GoogleOAuth", action = "Callback" }
            );
        }
    }
}