using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Threading;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    /// <summary>
    /// The redirect page for the Google OAuth process. Google should provide a code in the query string,
    /// which is used to obtain a reusable token. Redirects to <see cref="OAuthAuthorizationFinished"/> to
    /// display the results to the user.
    /// </summary>
    public partial class OAuthCallback : CMSPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var success = false;
            var message = String.Empty;
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();
            var code = QueryHelper.GetString("code", String.Empty);
            try
            {
                searchConsoleService.GoogleAuthorizationCodeFlow.ExchangeCodeForTokenAsync(
                    SearchConsoleConstants.DEFAULT_USER,
                    code,
                    searchConsoleService.GetUrlForCallback(),
                    CancellationToken.None
                ).ConfigureAwait(false).GetAwaiter().GetResult();

                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Service.Resolve<IEventLogService>().LogException(nameof(OAuthCallback), nameof(Page_Load), ex);
            }

            URLHelper.Redirect($"~/{SearchConsoleConstants.TOKEN_CALLBACK}?success={success}&message={message}");
        }
    }
}