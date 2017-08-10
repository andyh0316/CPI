using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpLocationDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            PhnomPenh = 1,
            Other = 2
        }
    }

    public class LookUpLocationMap : BaseMap<LookUpLocationDm>
    {
        public LookUpLocationMap()
        {
            ToTable("LookUp.Location");
        }
    }
}
