using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cpi.Application.DataModels
{
    public class InvoiceDm : BaseDm
    {
        [CpiRequired]
        public DateTime? Date { get; set; }

        [CpiRequired]
        [CpiMaxLength(20)]
        public string CustomerPhone { get; set; }

        [CpiMaxLength(20)]
        public string CustomerName { get; set; }

        [CpiMaxLength(100)]
        public string Address { get; set; }

        public virtual List<InvoiceCommodityDm> InvoiceCommodities { get; set; }

        public int? OperatorId { get; set; }
        public virtual UserDm Operator { get; set; }

        public int? DeliveryStaffId { get; set; }
        public virtual UserDm DeliveryStaff { get; set; }

        public int? StatusId { get; set; }
        public virtual LookUpInvoiceStatusDm Status { get; set; }

        public int? LocationId { get; set; }
        public virtual LookUpLocationDm Location { get; set; }

        public int? SourceId { get; set; }
        public virtual LookUpSourceDm Source { get; set; }

        public int? DeliveryDistanceId { get; set; }
        public virtual LookUpDeliveryDistanceDm DeliveryDistance { get; set; }

        [CpiRequired]
        [CpiGreaterThanZero]
        public decimal? TotalPrice { get; set; }

        public string Note { get; set; }
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
            HasOptional(a => a.Location).WithMany().HasForeignKey(a => a.LocationId).WillCascadeOnDelete(false);
            HasOptional(a => a.Source).WithMany().HasForeignKey(a => a.SourceId).WillCascadeOnDelete(false);
            HasOptional(a => a.DeliveryDistance).WithMany().HasForeignKey(a => a.DeliveryDistanceId).WillCascadeOnDelete(false);
        }
    }
}
