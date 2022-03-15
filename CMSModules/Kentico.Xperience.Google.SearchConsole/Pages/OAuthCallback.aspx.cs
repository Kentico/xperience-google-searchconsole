using CMS.Core;
using CMS.Helpers;

using Kentico.Xperience.Google.SearchConsole.Constants;
using Kentico.Xperience.Google.SearchConsole.Services;

using System;
using System.Threading;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    public partial class OAuthCallback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var success = false;
            var message = String.Empty;
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();

            var code = QueryHelper.GetString("code", String.Empty);
            var domainWithProtocol = $"{RequestContext.CurrentScheme}://{RequestContext.CurrentDomain}";
            try
            {
                searchConsoleService.GoogleAuthorizationCodeFlow.ExchangeCodeForTokenAsync(
                    "user",
                    code,
                    $"{domainWithProtocol}/{SearchConsoleConstants.OAUTH_CALLBACK}",
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