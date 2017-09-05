using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpPermissionDm : LookUpBaseDm
    {
        
    }

    public class LookUpPermissionMap : BaseMap<LookUpPermissionDm>
    {
        public LookUpPermissionMap()
        {
            ToTable("LookUp.LookUpPermission");
        }
    }
}
