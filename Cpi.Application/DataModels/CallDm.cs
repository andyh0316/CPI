using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Cpi.Application.DataModels
{
    public class CallDm : BaseDm
    {
        [CpiMaxLength(200)]
        public string CustomerName { get; set; }

        [CpiRequired]
        [CpiMaxLength(100)]
        [DisplayName("Customer Phone")]
        public string CustomerPhone { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public virtual List<CallCommodityDm> CallCommodities { get; set; }

        public int? OperatorId { get; set; }
        public virtual UserDm Operator { get; set; }

        public int? DeliveryStaffId { get; set; }
        public virtual UserDm DeliveryStaff { get; set; }

        [CpiMaxLength(300)]
        public string AddressString { get; set; }

        // we wont use this yet: maybe later
        [CpiRequired]
        public int? AddressId { get; set; }
        public virtual AddressDm Address { get; set; }

        public int? StatusId { get; set; }
        public virtual LookUpCallStatusDm Status { get; set; }

        [CpiRequired]
        public decimal? TotalPrice { get; set; }
    }

    public class CallMap : BaseMap<CallDm>
    {
        public CallMap()
        {
            ToTable("Call");

            HasMany(a => a.CallCommodities).WithRequired(a => a.Call).HasForeignKey(a => a.CallId).WillCascadeOnDelete(false);
            HasOptional(a => a.Operator).WithMany().HasForeignKey(a => a.OperatorId).WillCascadeOnDelete(false);
            HasOptional(a => a.DeliveryStaff).WithMany().HasForeignKey(a => a.DeliveryStaffId).WillCascadeOnDelete(false);
            HasRequired(a => a.Address).WithMany().HasForeignKey(a => a.AddressId).WillCascadeOnDelete(false);
            HasOptional(a => a.Status).WithMany().HasForeignKey(a => a.StatusId).WillCascadeOnDelete(false);
        }
    }
}
