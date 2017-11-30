using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels
{
    public class UserRolePermissionDm : BaseDm
    {
        [CpiRequired]
        public int? UserRoleId { get; set; }
        [JsonIgnore]
        public virtual LookUpUserRoleDm UserRole { get; set; }

        [CpiRequired]
        public int? PermissionId { get; set; }
        public virtual LookUpPermissionDm Permission { get; set; }

        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }

    public class UserRolePermissionMap : BaseMap<UserRolePermissionDm>
    {
        public UserRolePermissionMap()
        {
            ToTable("UserRolePermission");
            HasRequired(a => a.Permission).WithMany().HasForeignKey(a => a.PermissionId).WillCascadeOnDelete(false);
            HasRequired(a => a.UserRole).WithMany().HasForeignKey(a => a.UserRoleId).WillCascadeOnDelete(false);
        }
    }
}
