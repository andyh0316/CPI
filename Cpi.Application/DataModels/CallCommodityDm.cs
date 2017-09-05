using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;

namespace Cpi.Application.DataModels
{
    public class CallCommodityDm : BaseDm
    {
        public int CallId { get; set; }
        [JsonIgnore]
        public virtual CallDm Call { get; set; }

        public int CommodityId { get; set; }
        public virtual CommodityDm Commodity { get; set; }

        [CpiRequired]
        public int? Quantity { get; set; }
    }

    public class CallCommodityMap : BaseMap<CallCommodityDm>
    {
        public CallCommodityMap()
        {
            ToTable("CallCommodity");
            HasRequired(a => a.Call).WithMany().HasForeignKey(a => a.CallId).WillCascadeOnDelete(false);
            HasRequired(a => a.Commodity).WithMany().HasForeignKey(a => a.CommodityId).WillCascadeOnDelete(false);
        }
    }
}
