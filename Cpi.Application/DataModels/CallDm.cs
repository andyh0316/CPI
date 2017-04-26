using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cpi.Application.DataModels
{
    public class CallDm : BaseDm
    {
        [CpiMaxLength(200)]
        public string CustomerName { get; set; }

        [CpiRequired]
        [CpiMaxLength(100)]
        public string CustomerPhone { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [CpiRequiredOnCallStatus]
        public virtual List<CallCommodityDm> CallCommodities { get; set; }

        [CpiRequiredOnCallStatus]
        public int? OperatorId { get; set; }
        public virtual UserDm Operator { get; set; }

        public int? DeliveryStaffId { get; set; }
        public virtual UserDm DeliveryStaff { get; set; }

        [CpiMaxLength(300)]
        public string AddressString { get; set; }

        [CpiRequiredOnCallStatus]
        public int? StatusId { get; set; }
        public virtual LookUpCallStatusDm Status { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? CompletionDate { get; set; }
    }

    public class CallMap : BaseMap<CallDm>
    {
        public CallMap()
        {
            ToTable("Call");

            HasMany(a => a.CallCommodities).WithRequired(a => a.Call).HasForeignKey(a => a.CallId).WillCascadeOnDelete(false);
            HasOptional(a => a.Operator).WithMany().HasForeignKey(a => a.OperatorId).WillCascadeOnDelete(false);
            HasOptional(a => a.DeliveryStaff).WithMany().HasForeignKey(a => a.DeliveryStaffId).WillCascadeOnDelete(false);
            HasOptional(a => a.Status).WithMany().HasForeignKey(a => a.StatusId).WillCascadeOnDelete(false);
        }
    }
}
