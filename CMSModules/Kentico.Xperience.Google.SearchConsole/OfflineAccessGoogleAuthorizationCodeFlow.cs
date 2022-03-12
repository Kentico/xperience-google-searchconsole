using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;

using System;

namespace Kentico.Xperience.Google.SearchConsole
{
    public class OfflineAccessGoogleAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {

        public OfflineAccessGoogleAuthorizationCodeFlow(Initializer initializer) : base(initializer) { }


        public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
        {
            return new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
            {
                ClientId = ClientSecrets.ClientId,
                Scope = string.Join(" ", Scopes),
                RedirectUri = redirectUri,
                AccessType = "offline"
            };
        }
    };
}