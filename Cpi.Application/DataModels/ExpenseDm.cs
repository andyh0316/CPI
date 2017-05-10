using Cpi.Application.DataModels.Base;
using Cpi.Compass.Application.BusinessRules;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cpi.Application.DataModels
{
    public class ExpenseDm : BaseDm
    {
        [CpiRequired]
        [CpiMaxLength(100)]
        public string Name { get; set; }

        [CpiRequired]
        [CpiGreaterThanZero]
        public decimal? Expense { get; set; }

        [NotMapped]
        public decimal? PeriodBalance { get; set; }

        [NotMapped]
        public decimal? TotalBalance { get; set; }
    }

    public class ExpenseMap : BaseMap<ExpenseDm>
    {
        public ExpenseMap()
        {
            ToTable("Expense");
        }
    }
}
