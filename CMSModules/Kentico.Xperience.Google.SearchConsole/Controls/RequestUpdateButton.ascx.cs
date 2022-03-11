using System;
using System.Web.UI;

namespace Kentico.Xperience.Google.SearchConsole.Controls
{
    public partial class RequestUpdateButton : UserControl
    {
        public bool IsSingleNode
        {
            get;
            set;
        }


        public bool IsSection
        {
            get;
            set;
        }


        public bool IsContentTree
        {
            get;
            set;
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsSingleNode)
            {
                btnSubmit.Text = "Update selected page";
            }
            else if (IsSection)
            {
                btnSubmit.Text = "Update section";
            }
            else if (IsContentTree)
            {
                btnSubmit.Text = "Update content tree";
            }
            else
            {
                btnSubmit.Visible = false;
            }
        }
    }
}