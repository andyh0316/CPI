using Cpi.Application.DataModels.Base;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cpi.Application.DataModels
{
    public class CommodityDm : BaseDm
    {
        [CpiRequired]
        [CpiMaxLength(200)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual List<CallCommodityDm> CallCommodities { get; set; }

        [CpiRequired]
        public decimal? Price { get; set; }

        public bool Inactive { get; set; }
    }

    public class CommodityMap : BaseMap<CommodityDm>
    {
        public CommodityMap()
        {
            ToTable("Commodity");
            HasMany(a => a.CallCommodities).WithRequired(a => a.Commodity).HasForeignKey(a => a.CommodityId).WillCascadeOnDelete(false);
        }
    }
}
