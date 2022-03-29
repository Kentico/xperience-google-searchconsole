using CMS;
using CMS.Core;
using CMS.Helpers;
using CMS.SiteProvider;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Indexing.v3;
using Google.Apis.Indexing.v3.Data;
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
        private IEventLogService eventLogService;
        private IAppSettingsService appSettingsService;
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
            IEventLogService eventLogService,
            IAppSettingsService appSettingsService,
            IUrlInspectionStatusInfoProvider urlInspectionStatusInfoProvider)
        {
            this.eventLogService = eventLogService;
            this.appSettingsService = appSettingsService;
            this.urlInspectionStatusInfoProvider = urlInspectionStatusInfoProvider;
        }

        
        public string GetUrlForCallback()
        {
            var site = SiteInfo.Provider.Get()
                .WhereLike("SiteDomainName", $"{RequestContext.CurrentDomain}%")
                .FirstOrDefault();
            if (site != null)
            {
                return $"{RequestContext.CurrentScheme}://{site.DomainName}/{SearchConsoleConstants.OAUTH_CALLBACK}";
            }

            var siteAlias = SiteDomainAliasInfo.Provider.Get()
                .WhereEquals(nameof(SiteDomainAliasInfo.SiteDomainAliasType), SiteDomainAliasTypeEnum.Administration)
                .WhereLike(nameof(SiteDomainAliasInfo.SiteDomainAliasName), $"{RequestContext.CurrentDomain}%")
                .FirstOrDefault();
            if (siteAlias != null)
            {
                return $"{RequestContext.CurrentScheme}://{siteAlias.SiteDomainAliasName}/{SearchConsoleConstants.OAUTH_CALLBACK}";
            }

            return $"{RequestContext.CurrentScheme}://{SiteContext.CurrentSite.DomainName}/{SearchConsoleConstants.OAUTH_CALLBACK}";
        }


        public RequestResults GetInspectionResults(IEnumerable<string> urls, string cultureCode)
        {
            var requestResults = new RequestResults();
            var userCredential = GetUserCredential();
            if (userCredential == null)
            {
                var error = $"Unable to retrieve user credentials. Please ensure that client_secret.json and the authentication token are present in {SearchConsoleConstants.fileStorePhysicalPath}.";
                eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(GetInspectionResults), error);
                requestResults.FailedRequests = urls.Count();
                requestResults.Errors.Add(error);

                return requestResults;
            }

            var service = new SearchConsoleService(new BaseClientService.Initializer()
            {
                ApplicationName = appSettingsService[SearchConsoleConstants.APPSETTING_APPLICATIONNAME],
                HttpClientInitializer = userCredential
            });

            // Get search console formatted site URL
            var searchConsoleSite = GetSite(SiteContext.CurrentSite.SitePresentationURL);
            if (searchConsoleSite == null)
            {
                var error = $"Unable to retrieve the Google Search Console site. Please check the Event Log for details.";
                requestResults.FailedRequests = urls.Count();
                requestResults.Errors.Add(error);

                return requestResults;
            }

            var batch = new BatchRequest(service);
            foreach (var url in urls)
            {
                var request = new InspectUrlIndexRequest()
                {
                    InspectionUrl = url,
                    LanguageCode = cultureCode,
                    SiteUrl = searchConsoleSite.SiteUrl
                };

                batch.Queue<InspectUrlIndexResponse>(service.UrlInspection.Index.Inspect(request), (content, error, i, message) => {
                    if (!message.IsSuccessStatusCode)
                    {
                        requestResults.FailedRequests++;
                        requestResults.Errors.Add(error.Message);
                        eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(GetInspectionResults), error.Message);

                        return;
                    }

                    var existingInfo = urlInspectionStatusInfoProvider.Get()
                        .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                        .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), cultureCode)
                        .TopN(1)
                        .TypedResult
                        .FirstOrDefault();

                    if (existingInfo == null)
                    {
                        existingInfo = new UrlInspectionStatusInfo
                        {
                            Url = url,
                            Culture = cultureCode
                        };
                    }

                    existingInfo.InspectionResultRequestedOn = DateTime.Now;
                    existingInfo.LastInspectionResult = JsonConvert.SerializeObject(content);
                    urlInspectionStatusInfoProvider.Set(existingInfo);
                    requestResults.SuccessfulRequests++;
                });
            }

            batch.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return requestResults;
        }


        public UserCredential GetUserCredential()
        {
            if (GoogleAuthorizationCodeFlow == null)
            {
                return null;
            }

            var token = GoogleAuthorizationCodeFlow.LoadTokenAsync(SearchConsoleConstants.DEFAULT_USER, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            if (token == null)
            {
                return null;
            }

            var expiresOn = token.IssuedUtc.AddSeconds(ValidationHelper.GetDouble(token.ExpiresInSeconds, 0));
            if (DateTime.UtcNow >= expiresOn)
            {
                try
                {
                    GoogleAuthorizationCodeFlow.RefreshTokenAsync(SearchConsoleConstants.DEFAULT_USER, token.RefreshToken, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (TokenResponseException ex)
                {
                    eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(GetUserCredential), ex.Message);
                    return null;
                }
            }

            return new UserCredential(GoogleAuthorizationCodeFlow, SearchConsoleConstants.DEFAULT_USER, token);
        }


        public WmxSite GetSite(string xperienceDomain)
        {
            var userCredential = GetUserCredential();
            if (userCredential == null)
            {
                eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(GetSite),
                    $"Unable to retrieve user credentials. Please ensure that client_secret.json and the authentication token are present in {SearchConsoleConstants.fileStorePhysicalPath}.");
                return null;
            }

            var service = new SearchConsoleService(new BaseClientService.Initializer()
            {
                ApplicationName = appSettingsService[SearchConsoleConstants.APPSETTING_APPLICATIONNAME],
                HttpClientInitializer = userCredential
            });

            var response = service.Sites.List().Execute();
            var matchingSite = response.SiteEntry.FirstOrDefault(site => site.SiteUrl.Contains(xperienceDomain));
            if (matchingSite == null)
            {
                eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(GetSite), $"Couldn't find the site '{xperienceDomain}' within Google Search Console.");
            }

            return matchingSite;
        }


        private OfflineAccessGoogleAuthorizationCodeFlow InitializeAuthorizationFlow()
        {
            var credentialPath = $"{SearchConsoleConstants.fileStorePhysicalPath}\\{SearchConsoleConstants.CREDENTIALS_FILENAME}";
            if (!File.Exists(credentialPath))
            {
                eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(InitializeAuthorizationFlow), $"Google OAuth credential file '{SearchConsoleConstants.CREDENTIALS_FILENAME}' not found in the {SearchConsoleConstants.fileStorePhysicalPath} directory.");
                return null;
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


        public PublishUrlNotificationResponse RequestIndexingForPage(string url, string cultureCode)
        {
            var userCredential = GetUserCredential();
            if (userCredential == null)
            {
                eventLogService.LogError(nameof(DefaultSearchConsoleService), nameof(RequestIndexingForPage),
                    $"Unable to retrieve user credentials. Please ensure that client_secret.json and the authentication token are present in {SearchConsoleConstants.fileStorePhysicalPath}.");
                return null;
            }

            var service = new IndexingService(new BaseClientService.Initializer()
            {
                ApplicationName = appSettingsService[SearchConsoleConstants.APPSETTING_APPLICATIONNAME],
                HttpClientInitializer = userCredential
            });

            var request = service.UrlNotifications.Publish(new UrlNotification
            {
                Url = url,
                Type = "URL_UPDATED"
            });

            var response = request.Execute();
            if (response.UrlNotificationMetadata != null)
            {
                var existingInfo = urlInspectionStatusInfoProvider.Get()
                        .WhereEquals(nameof(UrlInspectionStatusInfo.Url), url)
                        .WhereEquals(nameof(UrlInspectionStatusInfo.Culture), cultureCode)
                        .TopN(1)
                        .TypedResult
                        .FirstOrDefault();

                if (existingInfo == null)
                {
                    existingInfo = new UrlInspectionStatusInfo
                    {
                        Url = url,
                        Culture = cultureCode
                    };
                }

                existingInfo.IndexingRequestedOn = DateTime.Now;
                urlInspectionStatusInfoProvider.Set(existingInfo);
            }

            return response;
        }
    }
}