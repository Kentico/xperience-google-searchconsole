using CMS;
using CMS.Core;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Indexing.v3;
using Google.Apis.Util.Store;

using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.IO;

[assembly: RegisterImplementation(typeof(ISearchConsoleService), typeof(DefaultSearchConsoleService), Lifestyle = Lifestyle.Singleton, Priority = RegistrationPriority.SystemDefault)]
namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public class DefaultSearchConsoleService : ISearchConsoleService
    {
        private GoogleAuthorizationCodeFlow mFlow;
        private ISettingsService settingsService;


        public GoogleAuthorizationCodeFlow GoogleAuthorizationCodeFlow
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


        public DefaultSearchConsoleService(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            /*UserCredential credential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { IndexingService.Scope.Indexing },
            "user",
            CancellationToken.None,
            new FileDataStore(fileStorePhysicalPath, true)
            ).ConfigureAwait(false).GetAwaiter().GetResult();
            }*/

            /*var service = new IndexingService(new BaseClientService.Initializer()
            {
                ApplicationName = "Dancing Goat",
                HttpClientInitializer = result.Credential
            });

            var request = service.UrlNotifications.GetMetadata();
            request.Url = "https://test.com/test";
            var response = request.Execute();*/
        }


        public TokenResponse GetAccessToken()
        {
            var tokenFromDb = settingsService[SearchConsoleConstants.SETTING_TOKEN];
            if (String.IsNullOrEmpty(tokenFromDb))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<TokenResponse>(tokenFromDb);
        }


        private GoogleAuthorizationCodeFlow InitializeAuthorizationFlow()
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
                    IndexingService.Scope.Indexing
                }
            };

            return new GoogleAuthorizationCodeFlow(initializer);
        }

    }
}