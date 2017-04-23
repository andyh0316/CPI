using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpCallStatusDm : LookUpBaseDm
    {
        public const int ID_COMPLETED = 1;
        public const int ID_DELIVERING = 2;
        public const int ID_CANCELLED = 3;
    }

    public class LookUpCallStatusMap : BaseMap<LookUpCallStatusDm>
    {
        public LookUpCallStatusMap()
        {
            ToTable("LookUp.CallStatus");
        }
    }
}
