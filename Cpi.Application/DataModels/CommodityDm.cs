using Cpi.Application.DataModels.Base;
using Cpi.Compass.Application.BusinessRules;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cpi.Application.DataModels
{
    public class CommodityDm : BaseDm
    {
        [CpiRequired]
        [CpiMaxLength(500)]
        public string Name { get; set; }

        [CpiRequired]
        public decimal? Price { get; set; }

        public bool Inactive { get; set; }
    }

    public class CommodityMap : BaseMap<CommodityDm>
    {
        public CommodityMap()
        {
            ToTable("Commodity");
        }
    }
}
