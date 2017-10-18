using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels
{
    public class UserPermissionDm : BaseDm
    {
        [CpiRequired]
        public int? PermissionId { get; set; }
        public virtual LookUpPermissionDm Permission { get; set; }

        [CpiRequired]
        public int? UserId { get; set; }
        public virtual UserDm User { get; set; }

        public bool View { get; set; }
        //public bool Create { get; set; }
    }

    public class UserPermissionMap : BaseMap<UserPermissionDm>
    {
        public UserPermissionMap()
        {
            ToTable("UserPermission");
            HasRequired(a => a.Permission).WithMany().HasForeignKey(a => a.PermissionId).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany().HasForeignKey(a => a.UserId).WillCascadeOnDelete(false);
        }
    }
}
