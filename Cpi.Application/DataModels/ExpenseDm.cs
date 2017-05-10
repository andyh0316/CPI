using Cpi.Application.DataModels.Base;
using Cpi.Compass.Application.BusinessRules;

namespace Cpi.Application.DataModels
{
    public class ExpenseDm : BaseDm
    {
        [CpiRequired]
        [CpiMaxLength(100)]
        public string Name { get; set; }

        [CpiRequired]
        public decimal? Expense { get; set; }
    }

    public class ExpenseMap : BaseMap<ExpenseDm>
    {
        public ExpenseMap()
        {
            ToTable("Expense");
        }
    }
}
