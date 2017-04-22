using Cpi.Application.DataModels.Base;
using Cpi.Compass.Application.BusinessRules;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpCallStatusDm : LookUpBaseDm
    {
    }

    public class LookUpCallStatusMap : BaseMap<LookUpCallStatusDm>
    {
        public LookUpCallStatusMap()
        {
            ToTable("LookUp.CallStatus");
        }
    }
}
