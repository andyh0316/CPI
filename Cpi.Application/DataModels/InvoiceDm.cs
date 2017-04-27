using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System.Collections.Generic;

namespace Cpi.Application.DataModels
{
    public class InvoiceDm : BaseDm
    {
        [CpiRequired]
        [CpiMaxLength(100)]
        public string CustomerPhone { get; set; }

        [CpiRequiredOnInvoiceStatus]
        public virtual List<InvoiceCommodityDm> InvoiceCommodities { get; set; }

        [CpiRequiredOnInvoiceStatus]
        public int? OperatorId { get; set; }
        public virtual UserDm Operator { get; set; }

        public int? DeliveryStaffId { get; set; }
        public virtual UserDm DeliveryStaff { get; set; }

        public int? StatusId { get; set; }
        public virtual LookUpInvoiceStatusDm Status { get; set; }

        public decimal? TotalPrice { get; set; }
    }

    public class InvoiceMap : BaseMap<InvoiceDm>
    {
        public InvoiceMap()
        {
            ToTable("Invoice");

            HasMany(a => a.InvoiceCommodities).WithRequired(a => a.Invoice).HasForeignKey(a => a.InvoiceId).WillCascadeOnDelete(false);
            HasOptional(a => a.Operator).WithMany().HasForeignKey(a => a.OperatorId).WillCascadeOnDelete(false);
            HasOptional(a => a.DeliveryStaff).WithMany().HasForeignKey(a => a.DeliveryStaffId).WillCascadeOnDelete(false);
            HasOptional(a => a.Status).WithMany().HasForeignKey(a => a.StatusId).WillCascadeOnDelete(false);
        }
    }
}
