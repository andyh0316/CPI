using Cpi.Application.DataModels.Base;
using System.Collections.Generic;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpUserRoleDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            Admin = 1,
            DataSpecialist = 2,
            Staff = 3,
            Laozi = 4
        }

        public virtual List<UserRolePermissionDm> UserRolePermissions { get; set; }
    }

    public class LookUpUserRoleMap : BaseMap<LookUpUserRoleDm>
    {
        public LookUpUserRoleMap()
        {
            ToTable("LookUp.UserRole");
            HasMany(m => m.UserRolePermissions).WithRequired(m => m.UserRole).HasForeignKey(m => m.UserRoleId).WillCascadeOnDelete(false);
        }
    }
}
