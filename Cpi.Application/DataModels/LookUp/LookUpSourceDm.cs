using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpSourceDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            HangMeas = 1,
            Facebook = 2,
        }
    }

    public class LookUpSourceMap : BaseMap<LookUpSourceDm>
    {
        public LookUpSourceMap()
        {
            ToTable("LookUp.Source");
        }
    }
}
