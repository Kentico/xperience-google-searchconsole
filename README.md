[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico) ![Kentico.Xperience.Libraries 13.0.0](https://img.shields.io/badge/Kentico.Xperience.Libraries-v13.0.0-orange)

# Xperience Google Search Console integration

This custom module allows Xperience users to view the indexed status of your Xperience website pages from [Google Search Console](https://search.google.com/search-console/about). Easily track which pages are included in your index and sitemaps, request page indexing, identify warnings and errors for specific pages, view reports for sections of your content tree, and create exports of issues to be fixed.

![Main screenshot](/Assets/mainscreenshot.png)

## Set up the environment

### Import the custom module

1. Open your CMS project in __Visual Studio__.
1. Install the _Google.Apis.SearchConsole.v1_ and _Google.Apis.Indexing.v3_ NuGet packages in the CMS project.
1. Download the latest export package from the [/CMSSiteUtils/Export](/CMSSiteUtils/Export) folder.
1. In the Xperience adminstration, open the __Sites__ application.
1. [Import](https://docs.xperience.io/deploying-websites/exporting-and-importing-sites/importing-a-site-or-objects) the downloaded package with the __Import files__ and __Import code files__ [settings](https://docs.xperience.io/deploying-websites/exporting-and-importing-sites/importing-a-site-or-objects#Importingasiteorobjects-Import-Objectselectionsettings) enabled.
1. Perform the [necessary steps](https://docs.xperience.io/deploying-websites/exporting-and-importing-sites/importing-a-site-or-objects#Importingasiteorobjects-Importingpackageswithfiles) to include the following imported folder in your project:
   - `/CMSModules/Kentico.Xperience.Google.SearchConsole`

### Authenticate with Google

Before you continue setting up the integration, ensure that your Xperience website is [registered as a property](https://support.google.com/webmasters/answer/34592) in Google Search Console.

1. Open the [Google Cloud Console](https://console.developers.google.com/).
1. Create a new project for your website, or select a pre-existing project.
1. Open the __APIs & Services → Library__ tab and enable the __Indexing API__ and __Google Search Console API__.
1. Open the __APIs & Services → Credentials__ tab.
1. Click __Create credentials → OAuth client ID__ and select "Web application."
1. Create a name, and under the __Authorized redirect URIs__ section, add the following URL: `https://<your Xperience admin>/CMSModules/Kentico.Xperience.Google.SearchConsole/Pages/OAuthCallback.aspx`.
1. After saving, click the __Download JSON__ button.
1. Rename the saved file to _client\_secrets.json_ and move it into your CMS application's _/CMS/App_Data/CMSModules/Kentico.Xperience.Google.SearchConsole_ folder (you will need to create this folder).
1. In the Xperience administration UI, open the new __Google search console__ application.
1. Click the __Authorize__ button and follow the process until you see a screen indicating that authentication was successful.

> Note: As you will see in a message on step #6, it may take Google up to 5 minutes for your new credentials to work. If you can't authenticate successfully, please wait and try again.

After successful authentication, you will see a new token file created in your application's  _/CMS/App_Data/CMSModules/Kentico.Xperience.Google.SearchConsole_ folder. This token will automatically refresh after it expires and is valid for all Xperience users, so you should only need to perform the authentication process once.