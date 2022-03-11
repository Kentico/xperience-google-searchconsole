using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using Kentico.Xperience.Google.SearchConsole;

[assembly: RegisterObjectType(typeof(PageIndexStatusInfo), PageIndexStatusInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// Data container class for <see cref="PageIndexStatusInfo"/>.
    /// </summary>
    [Serializable]
    public partial class PageIndexStatusInfo : AbstractInfo<PageIndexStatusInfo, IPageIndexStatusInfoProvider>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "googlesearchconsole.pageindexstatus";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(PageIndexStatusInfoProvider), OBJECT_TYPE, "GoogleSearchConsole.PageIndexStatus", "PageIndexStatusID", null, null, null, null, null, null, null, null)
        {
            ModuleName = "Kentico.Xperience.Google.SearchConsole",
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("NodeID", "cms.node", ObjectDependencyEnum.Required),
            },
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
        /// Node ID.
        /// </summary>
        [DatabaseField]
        public virtual int NodeID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("NodeID"), 0);
            }
            set
            {
                SetValue("NodeID", value, 0);
            }
        }


        /// <summary>
        /// Culture code.
        /// </summary>
        [DatabaseField]
        public virtual string CultureCode
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CultureCode"), String.Empty);
            }
            set
            {
                SetValue("CultureCode", value, String.Empty);
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
                SetValue("Url", value, String.Empty);
            }
        }


        /// <summary>
        /// Latest update.
        /// </summary>
        [DatabaseField]
        public virtual string LatestUpdate
        {
            get
            {
                return ValidationHelper.GetString(GetValue("LatestUpdate"), String.Empty);
            }
            set
            {
                SetValue("LatestUpdate", value, String.Empty);
            }
        }


        /// <summary>
        /// Latest remove.
        /// </summary>
        [DatabaseField]
        public virtual string LatestRemove
        {
            get
            {
                return ValidationHelper.GetString(GetValue("LatestRemove"), String.Empty);
            }
            set
            {
                SetValue("LatestRemove", value, String.Empty);
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
        protected PageIndexStatusInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="PageIndexStatusInfo"/> class.
        /// </summary>
        public PageIndexStatusInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="PageIndexStatusInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public PageIndexStatusInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}