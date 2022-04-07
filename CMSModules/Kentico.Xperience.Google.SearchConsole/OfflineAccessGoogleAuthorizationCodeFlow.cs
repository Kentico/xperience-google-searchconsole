using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;

using System;

namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// A custom <see cref="GoogleAuthorizationCodeFlow"/> implementation which provides access to the Google APIs
    /// without requiring user interaction to authenticate the request (once the token has been obtained).
    /// </summary>
    public class OfflineAccessGoogleAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineAccessGoogleAuthorizationCodeFlow"/> class.
        /// </summary>
        /// <param name="initializer">An initalizer containing scopes, client secrets, and a data store.</param>
        public OfflineAccessGoogleAuthorizationCodeFlow(Initializer initializer) : base(initializer) {
        
        }


        /// <summary>
        /// A custom implementation which sets the <see cref="GoogleAuthorizationCodeRequestUrl.AccessType"/> to
        /// "offline," allowing Google API access without user interaction.
        /// </summary>
        /// <param name="redirectUri">The URL to redirect to after Google OAuth authentication.</param>
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