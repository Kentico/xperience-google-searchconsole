using CMS.DataEngine;

namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// Class providing <see cref="PageIndexStatusInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IPageIndexStatusInfoProvider))]
    public partial class PageIndexStatusInfoProvider : AbstractInfoProvider<PageIndexStatusInfo, PageIndexStatusInfoProvider>, IPageIndexStatusInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageIndexStatusInfoProvider"/> class.
        /// </summary>
        public PageIndexStatusInfoProvider()
            : base(PageIndexStatusInfo.TYPEINFO)
        {
        }
    }
}