using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpCommodityDm : LookUpBaseDm
    {

    }

    public class LookUpCommodityMap : BaseMap<LookUpCommodityDm>
    {
        public LookUpCommodityMap()
        {
            ToTable("LookUp.Commodity");
        }
    }
}
