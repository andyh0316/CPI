using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpExpenseTypeDm : LookUpBaseDm
    {

    }

    public class LookUpExpenseTypeMap : BaseMap<LookUpExpenseTypeDm>
    {
        public LookUpExpenseTypeMap()
        {
            ToTable("LookUp.ExpenseType");
        }
    }
}
