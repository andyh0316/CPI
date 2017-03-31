using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels
{
    public class InvoiceDm : BaseDm
    {
        [MaxLength(200)]
        public string CustomerName { get; set; }

        [MaxLength(100)]
        public string CustomerPhone { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public virtual List<LookUpCommodityDm> Commodities { get; set; }
    }

    public class InvoiceMap : BaseMap<InvoiceDm>
    {
        public InvoiceMap()
        {
            ToTable("Invoice");
            HasMany<LookUpCommodityDm>(m => m.Commodities).WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("InvoiceId");
                    m.MapRightKey("CommodityId");
                    m.ToTable("InvoiceCommodity");
                });
        }
    }
}
