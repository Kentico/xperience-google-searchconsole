[![Stack Overflow](https://img.shields.io/badge/Stack%20Overflow-ASK%20NOW-FE7A16.svg?logo=stackoverflow&logoColor=white)](https://stackoverflow.com/tags/kentico) ![Kentico.Xperience.Libraries 13.0.0](https://img.shields.io/badge/Kentico.Xperience.Libraries-v13.0.0-orange)

# Xperience Google Search Console integration

This custom module allows Xperience users to view the indexed status of your Xperience website pages from [Google Search Console](https://search.google.com/search-console/about). Easily track which pages are included in your index and sitemaps, request page indexing, identify warnings and errors for specific pages, view reports for sections of your content tree, and create exports of issues to be fixed.

![Main screenshot](/Assets/mainscreenshot.png)

## Set up the environment

### Import the custom module

1. Open your CMS project in __Visual Studio__.
1. Install the _Google.Apis.SearchConsole.v1_ and _Google.Apis.Indexing.v3_ NuGet packages in the CMS project. This integration was tested using the __1.56.0.xx__ versions of both libraries.
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
1. Rename the saved file to _client\_secret.json_ and move it into your CMS application's _/CMS/App_Data/CMSModules/Kentico.Xperience.Google.SearchConsole_ folder (you will need to create this folder).
1. In the Xperience administration UI, open the new __Google search console__ application.
1. Click the __Authorize__ button and follow the process until you see a screen indicating that authentication was successful.

> Note: As you will see in a message on step #6, it may take Google up to 5 minutes for your new credentials to work. If you can't authenticate successfully, please wait and try again.

After successful authentication, you will see a new token file created in your application's  _/CMS/App_Data/CMSModules/Kentico.Xperience.Google.SearchConsole_ folder. This token will automatically refresh after it expires and is valid for all Xperience users, so you should only need to perform the authentication process once.

## Using the custom module

After [installing](#import-the-custom-module) and [authenticating](#authenticate-with-google), Xperience users can access the new __Google search console__ application found in the __Content management__ category. Permission to this application can be configured using standard [module permissions](https://docs.xperience.io/managing-users/configuring-permissions). The application's interface is divided into three sections:

![Module sections](/Assets/modulesections.png)

The __content tree__ displays all pages of your site and their overall Google Search Console indexing status. Pages that do not have live site URLs will not have an icon next to them. The possible icons are:

![Icon legend](/Assets/legend.png)

The __report__ section displays the details of the selected page's _direct_ children. The "Coverage," "Mobile usability," and "Rich results" columns will list any errors or warnings with the page, or a green checkmark if it is valid. Click the green "eye" icon to select that page from the content tree on the left and see its overview and report. The "arrow" icon will open the page's live site URL. You can export the details of these pages in multiple formats (e.g. Excel) by clicking the menu in the top-left of the grid. For example, if your articles have some mobile usability errors that need to be resolved by a front-end developer, you can export a full report of your article's issues as an Excel file and email it to your developers to be fixed!

The __overview__ section displays all Google Search Console data retrieved for the selected page only. You can also see the last time the data for the page was [refreshed](#refreshing-page-statuses) and when [indexing](#request-page-indexing) was requested.

## Refreshing page statuses

Google has [quotas](https://cloud.google.com/docs/quota) for their APIs and services which you should consider when using this integration. For example, the limitation for [the API we use](https://developers.google.com/webmaster-tools/limits#url-inspection) for refreshing is 600 queries per minute and 2,000 queries per day. To reduce the chances that your quota will be exceeded while using this integration, the data for each page is stored in the Xperience database and is _only_ retrieved from Google Search Console if you click the __Refresh__ buttons:

![Buttons](/Assets/refreshbuttons.png)

This means that, on the first time the application is accessed, all your pages will display a black question mark indicating that there's no data for the page. You will need to begin refreshing statuses for your pages, but make sure to keep the API quota in mind while doing so. Once refreshed, the data will not be updated until a new refresh is performed, but you can use the "Last refresh" column in the __report__ or "Status refreshed on" value in the __overview__ to check how stale the data is.

## Request page indexing

If a page isn't indexed in Google Search Console, or it needs to be re-indexed after a recent update, you can use the __Index__ buttons to request indexing:

![Index buttons](/Assets/indexbuttons.png)

Note that these buttons only _request_ that your pages be indexed, but the actual indexing by Google can take several days. After requesting indexing, you will need to wait a few days and either [refresh](#refreshing-page-statuses) the data, or view the status in Google Search Console directly. Similarly to refreshing statuses, [the API we use](https://developers.google.com/search/apis/indexing-api/v3/quota-pricing) for requesting page indexing has limitations. By default, you can make 200 indexing requests per day, so only request indexing when necessary, and be sure to check the "Indexing requested on" value on a page's __overview__ to make sure it hasn't already been requested recently.