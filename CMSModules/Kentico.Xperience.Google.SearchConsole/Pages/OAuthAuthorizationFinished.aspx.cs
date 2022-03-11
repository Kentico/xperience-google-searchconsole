using CMS.Helpers;
using CMS.UIControls;

using System;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    public partial class OAuthAuthorizationFinished : CMSModalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var success = QueryHelper.GetBoolean("success", false);
            if (!success)
            {
                var message = QueryHelper.GetString("message", String.Empty);
                pnlSuccess.Visible = false;
                pnlError.Visible = true;
                ltlErrorMessage.Text = message;
            }
        }
    }
}