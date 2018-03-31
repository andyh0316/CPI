using Cpi.Application.DataModels.Base;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpWeekDayDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }

        [JsonIgnore]
        public virtual List<UserDm> Users { get; set; }
    }

    public class LookUpWeekDayMap : BaseMap<LookUpWeekDayDm>
    {
        public LookUpWeekDayMap()
        {
            ToTable("LookUp.WeekDay");
        }
    }
}
