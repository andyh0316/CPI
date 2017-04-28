using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpUserOccupationDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            Operator = 1,
            Delivery = 2
        }
    }

    public class LookUpUserOccupationMap : BaseMap<LookUpUserOccupationDm>
    {
        public LookUpUserOccupationMap()
        {
            ToTable("LookUp.UserOccupation");
        }
    }
}
