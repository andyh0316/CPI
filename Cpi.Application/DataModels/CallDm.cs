using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.Collections.Generic;

namespace Cpi.Application.DataModels
{
    public class CallDm : BaseDm
    {
        [CpiRequired]
        public DateTime? Date { get; set; }

        [CpiMaxLength(200)]
        public string CustomerName { get; set; }

        [CpiRequired]
        [CpiMaxLength(100)]
        public string CustomerPhone { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [CpiMaxLength(300)]
        public string AddressString { get; set; }

        //[CpiRequiredOnCallStatus]
        public int? StatusId { get; set; }
        public virtual LookUpCallStatusDm Status { get; set; }
    }

    public class CallMap : BaseMap<CallDm>
    {
        public CallMap()
        {
            ToTable("Call");
            HasOptional(a => a.Status).WithMany().HasForeignKey(a => a.StatusId).WillCascadeOnDelete(false);
        }
    }
}
