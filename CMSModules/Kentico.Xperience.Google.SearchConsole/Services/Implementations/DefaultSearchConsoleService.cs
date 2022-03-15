using CMS;
using CMS.Core;
using CMS.Helpers;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Indexing.v3;
using Google.Apis.SearchConsole.v1;
using Google.Apis.SearchConsole.v1.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Kentico.Xperience.Google.SearchConsole.Models;
using Kentico.Xperience.Google.SearchConsole.Services;
using Kentico.Xperience.Google.SearchConsole.Constants;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

[assembly: RegisterImplementation(typeof(ISearchConsoleService), typeof(DefaultSearchConsoleService), Lifestyle = Lifestyle.Singleton, Priority = RegistrationPriority.SystemDefault)]
namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public class DefaultSearchConsoleService : ISearchConsoleService
    {
        private OfflineAccessGoogleAuthorizationCodeFlow mFlow;
        private ISettingsService settingsService;
        private IEventLogService eventLogService;
        private IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider;


        public OfflineAccessGoogleAuthorizationCodeFlow GoogleAuthorizationCodeFlow
        {
            get
            {
                if (mFlow == null)
                {
                    mFlow = InitializeAuthorizationFlow();
                }

                return mFlow;
            }
        }


        public DefaultSearchConsoleService(
            ISettingsService settingsService,
            IEventLogService eventLogService,
            IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider)
        {
            this.settingsService = settingsService;
            this.eventLogService = eventLogService;
            this.urlInspectionStatusInfoProvider = urlInspectionStatusInfoProvider;
        }


        public RequestResults GetInspectionResults(IEnumerable<string> urls, string cultureCode)
        {
            var userCredential = GetUserCredential();
            if (userCredential == null)
            {
                // TODO: Log error
            }

            var service = new SearchConsoleService(new BaseClientService.Initializer()
            {
                ApplicationName = "Dancing Goat",
                HttpClientInitializer = userCredential
            });

            // TODO: Configure SiteUrl properly as indicated by https://developers.google.com/webmaster-tools/v1/urlInspection.index/inspect#request-body
            var domainWithProtocol = $"{RequestContext.CurrentScheme}://{RequestContext.CurrentDomain}/";
            var batch = new BatchRequest(service);
            var requestResults = new RequestResults();
            foreach (var url in urls)
            {
                var request = new InspectUrlIndexRequest()
                {
                    InspectionUrl = url,
                    LanguageCode = cultureCode,
                    SiteUrl = domainWithProtocol
                };

                batch.Queue<InspectUrlIndexResponse>(service.UrlInspection.Index.Inspect(request), (content, error, i, message) => {
                    if (!message.IsSuccessStatusCode)
                    {
                        requestResults.FailedRequests++;
                        requestResults.Errors.Add(error.Message);
                        eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(GetInspectionResults), error.Message);
                    }
                    else
                    {
                        var existingInfo = urlInspectionStatusInfoProvider.Get()
                            .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                            .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), "en-US")
                            .TopN(1)
                            .TypedResult
                            .FirstOrDefault();

                        var urlInspectionStatusInfo = new UrlInspectionStatusInfo
                        {
                            Url = url,
                            Culture = cultureCode,
                            InspectionResultRequestedOn = DateTime.Now,
                            LastInspectionResult = JsonConvert.SerializeObject(content)
                        };

                        if (existingInfo != null)
                        {
                            urlInspectionStatusInfo.PageIndexStatusID = existingInfo.PageIndexStatusID;
                        }

                        urlInspectionStatusInfoProvider.Set(urlInspectionStatusInfo);
                        requestResults.SucessfulRequests++;
                    }
                });
            }

            batch.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return requestResults;
        }


        public UserCredential GetUserCredential()
        {
            var token = GoogleAuthorizationCodeFlow.LoadTokenAsync("user", CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            if (token == null)
            {
                return null;
            }

            var expiresOn = token.IssuedUtc.AddSeconds(ValidationHelper.GetDouble(token.ExpiresInSeconds, 0));
            if (DateTime.UtcNow >= expiresOn)
            {
                GoogleAuthorizationCodeFlow.RefreshTokenAsync("user", token.RefreshToken, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return new UserCredential(GoogleAuthorizationCodeFlow, "user", token);
        }


        private OfflineAccessGoogleAuthorizationCodeFlow InitializeAuthorizationFlow()
        {
            var credentialPath = $"{SearchConsoleConstants.fileStorePhysicalPath}\\{SearchConsoleConstants.CREDENTIALS_FILENAME}";
            if (!File.Exists(credentialPath))
            {
                throw new InvalidOperationException($"Google OAuth credential file '{SearchConsoleConstants.CREDENTIALS_FILENAME}' not found in the {SearchConsoleConstants.fileStorePhysicalPath} directory.");
            }

            GoogleAuthorizationCodeFlow.Initializer initializer;
            GoogleClientSecrets credentials;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credentials = GoogleClientSecrets.FromStream(stream);
            }

            initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                IncludeGrantedScopes = true,
                ClientSecrets = credentials.Secrets,
                DataStore = new FileDataStore(SearchConsoleConstants.fileStorePhysicalPath, true),
                Scopes = new string[]
                {
                    IndexingService.Scope.Indexing,
                    SearchConsoleService.Scope.WebmastersReadonly
                }
            };

            return new OfflineAccessGoogleAuthorizationCodeFlow(initializer);
        }

    }
}