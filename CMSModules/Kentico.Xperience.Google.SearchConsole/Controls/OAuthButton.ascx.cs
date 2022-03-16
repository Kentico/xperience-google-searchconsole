using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;

using Google.Apis.Auth.OAuth2.Web;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Threading;
using System.Web.UI;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class OAuthButton : UserControl
    {
        private ISearchConsoleService searchConsoleService;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            searchConsoleService = Service.Resolve<ISearchConsoleService>();
            if (searchConsoleService.GetUserCredential() == null)
            {
                InitButton();
            }
            else
            {
                pnlMain.Visible = false;
            }
        }


        private void InitButton()
        {
            var domainWithProtocol = $"{RequestContext.CurrentScheme}://{RequestContext.CurrentDomain}";
            var auth = new AuthorizationCodeWebApp(searchConsoleService.GoogleAuthorizationCodeFlow, $"{domainWithProtocol}/{SearchConsoleConstants.OAUTH_CALLBACK}", Guid.NewGuid().ToString());
            var result = auth.AuthorizeAsync(SearchConsoleConstants.DEFAULT_USER, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

            var script = $"function AuthorizeGoogle() {{ window.open('{result.RedirectUri}', '', 'width=400,height=600,left=700,top=200'); }}";
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "AuthorizeGoogle", script, true);

            btnAuth.OnClientClick = "AuthorizeGoogle();";
        }
    }
}