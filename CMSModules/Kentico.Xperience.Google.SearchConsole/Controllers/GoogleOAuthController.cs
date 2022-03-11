using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;

namespace Kentico.Xperience.Google.SearchConsole.Controllers
{
    public class GoogleOAuthController : ApiController
    {
        [HttpGet]
        public RedirectResult Callback()
        {
            var success = false;
            var message = String.Empty;
            var searchConsoleService = Service.Resolve<ISearchConsoleService>();

            var code = QueryHelper.GetString("code", String.Empty);
            var domainWithProtocol = $"{CMS.Helpers.RequestContext.CurrentScheme}://{CMS.Helpers.RequestContext.CurrentDomain}";
            try
            {
                var tokenResponse = searchConsoleService.GoogleAuthorizationCodeFlow.ExchangeCodeForTokenAsync(
                    "user",
                    code,
                    $"{domainWithProtocol}/{SearchConsoleConstants.OAUTH_CALLBACK}",
                    new CancellationToken()
                ).ConfigureAwait(false).GetAwaiter().GetResult();

                var tokenData = JsonConvert.SerializeObject(tokenResponse);
                SettingsKeyInfoProvider.SetGlobalValue(SearchConsoleConstants.SETTING_TOKEN, tokenData, false);

                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Service.Resolve<IEventLogService>().LogException(nameof(GoogleOAuthController), nameof(Callback), ex);
            }

            return Redirect($"{domainWithProtocol}/{SearchConsoleConstants.TOKEN_CALLBACK}?success={success}&message={message}");
        }
    }
}