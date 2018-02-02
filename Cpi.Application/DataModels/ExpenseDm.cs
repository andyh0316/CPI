using Cpi.Application.DataModels.Base;
using Cpi.Application.DataModels.LookUp;
using Cpi.Compass.Application.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cpi.Application.DataModels
{
    public class ExpenseDm : BaseDm
    {
        [CpiRequired]
        public DateTime? Date { get; set; }

        [CpiRequired]
        [CpiMaxLength(100)]
        public string Name { get; set; }

        [CpiRequired]
        [CpiGreaterThanZero]
        public decimal? Expense { get; set; }

        public int? LocationId { get; set; }
        public virtual LookUpLocationDm Location { get; set; }

        public int? HandledById { get; set; }
        public virtual UserDm HandledBy { get; set; }

        public string Note { get; set; }
    }

    public class ExpenseMap : BaseMap<ExpenseDm>
    {
        public ExpenseMap()
        {
            ToTable("Expense");
            HasOptional(a => a.Location).WithMany().HasForeignKey(a => a.LocationId).WillCascadeOnDelete(false);
            HasOptional(a => a.HandledBy).WithMany().HasForeignKey(a => a.HandledById).WillCascadeOnDelete(false);
        }
    }
}
