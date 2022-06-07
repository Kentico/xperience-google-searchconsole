[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico) ![Kentico.Xperience.Libraries 13.0.0](https://img.shields.io/badge/Kentico.Xperience.Libraries-v13.0.0-orange)

# Xperience Google Search Console integration

This custom module allows Xperience users to view the indexed status of your Xperience website pages from [Google Search Console](https://search.google.com/search-console/about). Easily track which pages are included in your index and sitemaps, request page indexing, identify warnings and errors for specific pages, view reports for sections of your content tree, and create exports of issues to be fixed.

![Main screenshot](/Assets/mainscreenshot.png)

## Set up the environment

### Import the custom module

1. Open your CMS project in __Visual Studio__.
1. Install the _Google.Apis.SearchConsole.v1_ and _Google.Apis.Indexing.v3_ NuGet packages into the CMS project. This integration was tested using the __1.56.0.xx__ versions of both libraries.
1. Download the latest _"Kentico.Xperience.Google.SearchConsole"_ package from the [Releases](https://github.com/Kentico/xperience-google-searchconsole/releases).
1. In the Xperience adminstration, open the __Sites__ application.
1. [Import](https://docs.xperience.io/x/VAeRBg) the downloaded package with the __Import files__ and __Import code files__ [settings](https://docs.xperience.io/x/VAeRBg#Importingasiteorobjects-Import-Objectselectionsettings) enabled.
1. Perform the [necessary steps](https://docs.xperience.io/x/VAeRBg#Importingasiteorobjects-Importingpackageswithfiles) to include the following imported folder in your project:
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
1. Rename the saved file to _client\_secret.json_ and move it into your CMS application's _/CMS/App_Data/CMSModules/Kentico.Xperience.Google.SearchConsole_ folder (you will need to create this folder).
1. In the Xperience administration UI, open the new __Google Search Console__ application.
1. Click the __Authorize__ button and follow the process until you see a screen indicating that authentication was successful.

> Note: As you can see in a message on step #6, it may take up to 5 minutes for Google to register your new credentials. If you can't authenticate successfully, wait and try again.

A successful authentication creates a new token in your application's _/CMS/App_Data/CMSModules/Kentico.Xperience.Google.SearchConsole_ folder. This token automatically refreshes and is valid for all Xperience users - you only need to authenticate once per project.

## Using the custom module

After [installing](#import-the-custom-module) and [authenticating](#authenticate-with-google), Xperience users can access the new __Google Search Console__ application located under the __Content management__ category. Permissions for this application can be configured using standard [module permissions](https://docs.xperience.io/x/kgmRBg). The application's interface is divided into the following sections:

![Module sections](/Assets/modulesections.png)

The __Content tree__ displays all pages of your site and their overall Google Search Console indexing status. Pages that don't have live site URLs are displayed without an icon next to them. A black "minus" icon indicates the data hasn't been [refreshed](#refreshing-page-data) yet. If the data has been refreshed, you will see a green, yellow, or red icon indicating the indexed status of the page.

The __Report__ section displays the details of the selected page's _direct_ children. The __Coverage__, __Mobile usability__, and __Rich results__ columns list any errors or warnings with the page, or display a green checkmark if the page is valid. The green "eye" icon selects that page from the content tree on the left and displays its overview and report. The "arrow" icon opens the page's live site URL. You can export the details of these pages in multiple formats (e.g., Excel) by clicking the menu in the top-left of the grid. For example, if your articles have some mobile usability errors that need to be resolved by a front-end developer, you can export a full report of your article's issues as an Excel file and email it to your developers to be fixed!

The __Overview__ section displays all Google Search Console data retrieved for the selected page only. You can also see the last time the data for the page was [refreshed](#refreshing-page-data) and [indexing](#request-page-indexing) requested.

## Refreshing page data

Google has [quotas](https://cloud.google.com/docs/quota) for their APIs and services which you should consider when using this integration. For example, the limitation for [the API we use](https://developers.google.com/webmaster-tools/limits#url-inspection) to refresh page data is 600 queries per minute and 2,000 queries per day. To reduce the chances you exceed these quotas while using this integration, the data for each page is stored in the Xperience database and is _only_ retrieved from Google Search Console if you use the __Refresh data__ action below the report.

This means that when the __Google Search Console__ application is first accessed, all your pages display a black "minus" icon indicating that there's no data for the page stored in the project's database. You need to begin refreshing data for your pages while keeping the API quota in mind. Once refreshed, the data isn't updated until a new refresh is requested. You can use the __Last data refresh__ column in the _Report_ section or the __Data refreshed on__ value in the _Overview_ section to check how stale the data is.

## Request page indexing

If a page isn't indexed in Google Search Console, or it needs to be re-indexed after a recent update, you can use the __Request indexing__ action below the report to submit a request for re-indexing to Google.

Note that this only _request_ that your pages be indexed, but the actual indexing by Google can take several days. After you request re-indexing, you need to wait a few days and either [refresh](#refreshing-page-data) the data, or view the indexing status in Google Search Console directly. Similarly to refreshing statuses, [the API we use](https://developers.google.com/search/apis/indexing-api/v3/quota-pricing) for requesting page indexing has limitations. By default, you can make 200 indexing requests per day, so only request indexing when necessary, and check the __Indexing requested on__ value on a page's _Overview_ to make sure indexing has not been requested recently.
