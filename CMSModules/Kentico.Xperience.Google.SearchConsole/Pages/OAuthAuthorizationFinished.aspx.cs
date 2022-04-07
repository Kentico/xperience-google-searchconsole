using CMS.Helpers;
using CMS.UIControls;

using System;

namespace Kentico.Xperience.Google.SearchConsole.Pages
{
    /// <summary>
    /// A dialog page that the user is redirected to after the <see cref="OAuthCallback"/> page verifies
    /// the Google OAuth response and saves the token. Displays a success or error message to the user.
    /// </summary>
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