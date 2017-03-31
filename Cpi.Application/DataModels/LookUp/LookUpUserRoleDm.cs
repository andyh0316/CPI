using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpUserRoleDm : LookUpBaseDm
    {
        public const int LOOKUP_ADMIN_ID = 1;

    }

    public class LookUpUserRoleMap : BaseMap<LookUpUserRoleDm>
    {
        public LookUpUserRoleMap()
        {
            ToTable("LookUp.UserRole");
        }
    }
}
