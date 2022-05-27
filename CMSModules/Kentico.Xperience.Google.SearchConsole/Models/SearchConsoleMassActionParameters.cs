using Kentico.Xperience.Google.SearchConsole.Pages;

using System.Collections.Generic;

namespace Kentico.Xperience.Google.SearchConsole.Models
{
    /// <summary>
    /// Parameters that are stored in session and retrieved when performing a mass action
    /// with <see cref="SearchConsoleReportMassAction"/>.
    /// </summary>
    public class SearchConsoleMassActionParameters
    {
        /// <summary>
        /// The name of the action being performed.
        /// </summary>
        public string ActionName
        {
            get;
            set;
        }


        /// <summary>
        /// The node IDs to perform the action on.
        /// </summary>
        public IEnumerable<int> NodeIDs
        {
            get;
            set;
        }


        /// <summary>
        /// The culture of the <see cref="NodeIDs"/> to retrive.
        /// </summary>
        public string Culture
        {
            get;
            set;
        }


        /// <summary>
        /// The title to be displayed in the modal window.
        /// </summary>
        public string Title
        {
            get;
            set;
        }


        /// <summary>
        /// The javascript to call when the modal window is closed.
        /// </summary>
        public string ReloadScript
        {
            get;
            set;
        }
    }
}