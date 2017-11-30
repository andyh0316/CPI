using Cpi.Application.DataModels.Base;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpPermissionDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            Call = 1,
            Invoice = 2,
            Expense = 3,
            FinanceOverview = 4,
            FinanceList = 5,
            Performance = 6,
            User = 7,
            Commodity = 8
        }

        public enum ActionIds
        {
            Create = 1,
            Edit = 2,
            Delete = 3
        }
    }

    public class LookUpPermissionMap : BaseMap<LookUpPermissionDm>
    {
        public LookUpPermissionMap()
        {
            ToTable("LookUp.Permission");
        }
    }
}
