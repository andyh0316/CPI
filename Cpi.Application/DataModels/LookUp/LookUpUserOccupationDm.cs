using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpUserOccupationDm : LookUpBaseDm
    {
    }

    public class LookUpUserOccupationMap : BaseMap<LookUpUserOccupationDm>
    {
        public LookUpUserOccupationMap()
        {
            ToTable("LookUp.UserOccupation");
        }
    }
}
