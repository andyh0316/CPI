using Cpi.Application.DataModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataModels.LookUp
{
    public class LookUpDeliveryDistanceDm : LookUpBaseDm
    {
        public enum LookUpIds
        {
            _0_to_15 = 1,
            _15_to_30 = 2,
            _30_to_40 = 3,
            _40_or_more = 4
        }
    }

    public class LookUpDeliveryDistanceMap : BaseMap<LookUpDeliveryDistanceDm>
    {
        public LookUpDeliveryDistanceMap()
        {
            ToTable("LookUp.DeliveryDistance");
        }
    }
}
