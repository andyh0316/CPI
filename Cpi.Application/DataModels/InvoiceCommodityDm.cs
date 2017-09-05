using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;

namespace Cpi.Application.DataModels
{
    public class InvoiceCommodityDm : BaseDm
    {
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public virtual InvoiceDm Invoice { get; set; }

        public int CommodityId { get; set; }
        public virtual CommodityDm Commodity { get; set; }

        [CpiRequired]
        public int? Quantity { get; set; }
    }

    public class InvoiceCommodityMap : BaseMap<InvoiceCommodityDm>
    {
        public InvoiceCommodityMap()
        {
            ToTable("InvoiceCommodity");
            HasRequired(a => a.Invoice).WithMany().HasForeignKey(a => a.InvoiceId).WillCascadeOnDelete(false);
            HasRequired(a => a.Commodity).WithMany().HasForeignKey(a => a.CommodityId).WillCascadeOnDelete(false);
        }
    }
}
