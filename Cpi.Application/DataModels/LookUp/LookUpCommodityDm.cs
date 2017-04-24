using Cpi.Application.DataModels.Base;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpCommodityDm : LookUpBaseDm
    {
        [JsonIgnore]
        public virtual List<CallCommodityDm> CallCommodities { get; set; }

        [CpiRequired]
        public decimal? Price { get; set; }

        public bool Inactive { get; set; }
    }

    public class LookUpCommodityMap : BaseMap<LookUpCommodityDm>
    {
        public LookUpCommodityMap()
        {
            ToTable("LookUp.Commodity");
            HasMany(a => a.CallCommodities).WithRequired(a => a.Commodity).HasForeignKey(a => a.CommodityId).WillCascadeOnDelete(false);
        }
    }
}
