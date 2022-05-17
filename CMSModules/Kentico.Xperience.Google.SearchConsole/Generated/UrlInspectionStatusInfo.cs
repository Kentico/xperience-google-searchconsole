using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Google.SearchConsole;

[assembly: RegisterObjectType(typeof(UrlInspectionStatusInfo), UrlInspectionStatusInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// Data container class for <see cref="UrlInspectionStatusInfo"/>.
    /// </summary>
    [Serializable]
    public partial class UrlInspectionStatusInfo : AbstractInfo<UrlInspectionStatusInfo, IUrlInspectionStatusInfoProvider>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "googlesearchconsole.urlinspectionstatus";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(UrlInspectionStatusInfoProvider), OBJECT_TYPE, "GoogleSearchConsole.UrlInspectionStatus", "PageIndexStatusID", null, null, null, null, null, null, null, null)
        {
            ModuleName = "Kentico.Xperience.Google.SearchConsole",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Page index status ID.
        /// </summary>
        [DatabaseField]
        public virtual int PageIndexStatusID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("PageIndexStatusID"), 0);
            }
            set
            {
                SetValue("PageIndexStatusID", value);
            }
        }


        /// <summary>
        /// Culture.
        /// </summary>
        [DatabaseField]
        public virtual string Culture
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Culture"), String.Empty);
            }
            set
            {
                SetValue("Culture", value);
            }
        }


        /// <summary>
        /// Inspection result requested on.
        /// </summary>
        [DatabaseField]
        public virtual DateTime InspectionResultRequestedOn
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("InspectionResultRequestedOn"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("InspectionResultRequestedOn", value, DateTimeHelper.ZERO_TIME);
            }
        }


        /// <summary>
        /// Url.
        /// </summary>
        [DatabaseField]
        public virtual string Url
        {
            get
            {
                return ValidationHelper.GetString(GetValue("Url"), String.Empty);
            }
            set
            {
                SetValue("Url", value);
            }
        }


        /// <summary>
        /// Last inspection result.
        /// </summary>
        [DatabaseField]
        public virtual string LastInspectionResult
        {
            get
            {
                return ValidationHelper.GetString(GetValue("LastInspectionResult"), String.Empty);
            }
            set
            {
                SetValue("LastInspectionResult", value, String.Empty);
            }
        }


        /// <summary>
        /// Indexing requested on.
        /// </summary>
        [DatabaseField]
        public virtual DateTime IndexingRequestedOn
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("IndexingRequestedOn"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("IndexingRequestedOn", value, DateTimeHelper.ZERO_TIME);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected UrlInspectionStatusInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="UrlInspectionStatusInfo"/> class.
        /// </summary>
        public UrlInspectionStatusInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="UrlInspectionStatusInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public UrlInspectionStatusInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}