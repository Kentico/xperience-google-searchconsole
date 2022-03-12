﻿using CMS;
using CMS.Core;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Indexing.v3;
using Google.Apis.Indexing.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Kentico.Xperience.Google.SearchConsole.Services;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

using static Google.Apis.Indexing.v3.UrlNotificationsResource;

[assembly: RegisterImplementation(typeof(ISearchConsoleService), typeof(DefaultSearchConsoleService), Lifestyle = Lifestyle.Singleton, Priority = RegistrationPriority.SystemDefault)]
namespace Kentico.Xperience.Google.SearchConsole.Services
{
    public class DefaultSearchConsoleService : ISearchConsoleService
    {
        private OfflineAccessGoogleAuthorizationCodeFlow mFlow;
        private ISettingsService settingsService;


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


        public DefaultSearchConsoleService(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
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


        public void GetMetadata(IEnumerable<string> urls)
        {
            var userCredential = GetUserCredential();
            if (userCredential == null)
            {
                // TODO: Log error
            }

            var service = new IndexingService(new BaseClientService.Initializer()
            {
                ApplicationName = "Dancing Goat",
                HttpClientInitializer = userCredential
            });

            var batch = new BatchRequest(service);
            foreach (var url in urls)
            {
                var metadataRequest = new GetMetadataRequest(service) { Url = url };
                batch.Queue<UrlNotificationMetadata>(metadataRequest, (content, error, i, message) => {
                    // TODO: Handle response
                });
            }

            batch.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public UserCredential GetUserCredential()
        {
            var token = GetAccessToken();
            if (token == null)
            {
                return null;
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
                    IndexingService.Scope.Indexing
                }
            };

            return new OfflineAccessGoogleAuthorizationCodeFlow(initializer);
        }

    }
}