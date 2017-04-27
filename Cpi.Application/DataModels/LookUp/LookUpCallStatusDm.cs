using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpCallStatusDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            SentToCallCenter = 1
        }
    }

    public class LookUpCallStatusMap : BaseMap<LookUpCallStatusDm>
    {
        public LookUpCallStatusMap()
        {
            ToTable("LookUp.CallStatus");
        }
    }
}
