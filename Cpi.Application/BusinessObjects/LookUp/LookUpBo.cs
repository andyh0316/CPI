using Cpi.Application.BusinessObjects.Base;
using Cpi.Application.DatabaseContext;
using Cpi.Application.DataModels.Base;
using Cpi.Application.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cpi.Application.BusinessObjects.LookUp
{
    public class LookUpBo
    {
        public CpiDbContext CpiDbContext
        {
            get
            {
                if (HttpContext.Current.Items["SharedDbContextForRequest"] != null)
                {
                    return ((CpiDbContext)HttpContext.Current.Items["SharedDbContextForRequest"]);
                }
                else
                {
                    CpiDbContext newCpiDbContext = new CpiDbContext();
                    HttpContext.Current.Items["SharedDbContextForRequest"] = newCpiDbContext;
                    return newCpiDbContext;
                }
            }
        }

        public IQueryable<T> GetListQuery<T>() where T : LookUpBaseDm
        {
            return CpiDbContext.Set<T>().OrderBy(a => a.DisplayOrder);
        }

        public List<T> GetList<T>() where T : LookUpBaseDm
        {
            return GetListQuery<T>().ToList();
        }

        public List<LookUpBaseDm> GetListWithNullOption<T>() where T : LookUpBaseDm
        {
            List<LookUpBaseDm> lookUps = GetListQuery<T>().ToList().Cast<LookUpBaseDm>().ToList();
            //lookUps.Insert(0, new LookUpBaseDm
            //{
            //    Id = ConstHelper.DROP_DOWN_NULL_ID,
            //    Name = "haha"
            //});

            return lookUps;
        }
    }
}
