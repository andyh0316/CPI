using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.BusinessObjects
{
    public class CommodityBo : BaseBo<CommodityDm>
    {
        public IQueryable<CommodityDm> GetListBaseQuery()
        {
            IQueryable<CommodityDm> query = GetListQuery();
            return query;
        }
    }
}