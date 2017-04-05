using Cpi.Application.DataModels.Base;

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
