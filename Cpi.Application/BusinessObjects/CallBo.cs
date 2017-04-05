using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DataModels;
using System.Linq;

namespace Cpi.Application.BusinessObjects
{
    public class CallBo : BaseBo<CallDm>
    {
        public IQueryable<CallDm> GetListBaseQuery()
        {
            return GetListQuery();
        }
    }
}
