using CMS.Base.Web.UI;

namespace Kentico.Xperience.Google.SearchConsole
{
    public class IconSet
    {
        public static string Checked(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-check-circle", tooltip, additionalClass: "tn color-green-100");
        }
        
        
        public static string Error(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-times-circle", tooltip, additionalClass: "tn color-red-70");
        }
        
        
        public static string Warning(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", tooltip, additionalClass: "tn color-orange-80");
        }


        public static string Minus(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-minus-circle", tooltip, additionalClass: "tn color-blue-100");
        }
        
        
        public static string Question(string tooltip)
        {
            return UIHelper.GetAccessibleIconTag("icon-question-circle", tooltip, additionalClass: "tn color-gray-50");
        }
    }
}