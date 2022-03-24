using CMS.Base.Web.UI;
using CMS.Core;

using Google.Apis.Auth.OAuth2.Web;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Threading;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class OAuthButton : AbstractUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitButton();
        }


        private void InitButton()
        {
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            if (searchConsoleService.GoogleAuthorizationCodeFlow == null)
            {
                ShowError("Error initializing Google OAuth process. Please check the Event Log for more information.");
                btnAuth.Enabled = false;
                return;
            }

            var auth = new AuthorizationCodeWebApp(searchConsoleService.GoogleAuthorizationCodeFlow, searchConsoleService.GetUrlForCallback(), Guid.NewGuid().ToString());
            var result = auth.AuthorizeAsync(SearchConsoleConstants.DEFAULT_USER, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

            var script = $"function AuthorizeGoogle() {{ window.open('{result.RedirectUri}', '', 'width=400,height=600,left=700,top=200'); }}";
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "AuthorizeGoogle", script, true);

            btnAuth.OnClientClick = "AuthorizeGoogle();";
        }
    }
}