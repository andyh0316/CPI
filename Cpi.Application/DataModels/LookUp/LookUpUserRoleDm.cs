using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpUserRoleDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            Admin = 1,
            DataSpecialist = 2,
            Staff = 3
        }
    }

    public class LookUpUserRoleMap : BaseMap<LookUpUserRoleDm>
    {
        public LookUpUserRoleMap()
        {
            ToTable("LookUp.UserRole");
        }
    }
}
