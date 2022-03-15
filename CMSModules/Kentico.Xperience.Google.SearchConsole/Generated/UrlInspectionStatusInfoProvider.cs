using CMS.DataEngine;

namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// Class providing <see cref="UrlInspectionStatusInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IUrlInspectionStatusInfoProvider))]
    public partial class UrlInspectionStatusInfoProvider : AbstractInfoProvider<UrlInspectionStatusInfo, UrlInspectionStatusInfoProvider>, IUrlInspectionStatusInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlInspectionStatusInfoProvider"/> class.
        /// </summary>
        public UrlInspectionStatusInfoProvider()
            : base(UrlInspectionStatusInfo.TYPEINFO)
        {
        }
    }
}