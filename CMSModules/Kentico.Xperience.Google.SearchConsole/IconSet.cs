using CMS.Base.Web.UI;

namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// Contains methods to retrieve commonly used font icons within the Google Search Console integration.
    /// </summary>
    public class IconSet
    {
        /// <summary>
        /// Gets a font icon indicating success.
        /// </summary>
        /// <param name="tooltip">The tooltip to display on hover.</param>
        public static string Success(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-check-circle", tooltip, additionalClass: "tn color-green-100");
        }


        /// <summary>
        /// Gets a font icon indicating an error.
        /// </summary>
        /// <param name="tooltip">The tooltip to display on hover.</param>
        public static string Error(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-times-circle", tooltip, additionalClass: "tn color-red-70");
        }


        /// <summary>
        /// Gets a font icon indicating a warning.
        /// </summary>
        /// <param name="tooltip">The tooltip to display on hover.</param>
        public static string Warning(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", tooltip, additionalClass: "tn color-orange-80");
        }


        /// <summary>
        /// Gets a font icon indicating an unimportant result.
        /// </summary>
        /// <param name="tooltip">The tooltip to display on hover.</param>
        public static string Ambiguous(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-minus-circle", tooltip, additionalClass: "tn color-blue-100");
        }


        /// <summary>
        /// Gets a font icon indicating an unknown result.
        /// </summary>
        /// <param name="tooltip">The tooltip to display on hover.</param>
        public static string Unknown(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-question-circle", tooltip, additionalClass: "tn color-gray-50");
        }
    }
}