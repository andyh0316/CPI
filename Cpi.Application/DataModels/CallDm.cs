using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Cpi.Application.DataModels
{
    public class CallDm : BaseDm
    {
        [MaxLength(200)]
        public string CustomerName { get; set; }

        [MaxLength(100)]
        public string CustomerPhone { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public virtual List<LookUpCommodityDm> Commodities { get; set; }

        public int? OperatorId { get; set; }
        public virtual UserDm Operator { get; set; }

        public int? DeliveryStaffId { get; set; }
        public virtual UserDm DeliveryStaff { get; set; }

        [Required]
        public int? AddressId { get; set; }
        public virtual AddressDm Address { get; set; }

        public int? StatusId { get; set; }
        public virtual LookUpCallStatusDm Status { get; set; }
    }

    public class CallMap : BaseMap<CallDm>
    {
        public CallMap()
        {
            ToTable("Call");
            HasMany<LookUpCommodityDm>(m => m.Commodities).WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("CallId");
                    m.MapRightKey("CommodityId");
                    m.ToTable("CallCommodity");
                });

            HasOptional(a => a.Operator).WithMany().HasForeignKey(a => a.OperatorId).WillCascadeOnDelete(false);
            HasOptional(a => a.DeliveryStaff).WithMany().HasForeignKey(a => a.DeliveryStaffId).WillCascadeOnDelete(false);
            HasRequired(a => a.Address).WithMany().HasForeignKey(a => a.AddressId).WillCascadeOnDelete(false);
            HasOptional(a => a.Status).WithMany().HasForeignKey(a => a.StatusId).WillCascadeOnDelete(false);
        }
    }
}
