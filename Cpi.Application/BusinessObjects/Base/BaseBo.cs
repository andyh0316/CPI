using Cpi.Application.DatabaseContext;
using Cpi.Application.DataModels.Base;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace Cpi.Application.BusinessObjects.Base
{
    public class BaseBo<T> where T : BaseDm
    {
        //private CpiDbContext context;
        public DbSet<T> dbSet
        {
            get
            {
                if (HttpContext.Current.Items["SharedDbContextForRequest"] != null)
                {
                    return ((CpiDbContext)HttpContext.Current.Items["SharedDbContextForRequest"]).Set<T>();
                }
                else
                {
                    CpiDbContext newCpiDbContext = new CpiDbContext();
                    HttpContext.Current.Items["SharedDbContextForRequest"] = newCpiDbContext;
                    return newCpiDbContext.Set<T>();
                } 
            }
        }

        // CAREFUL: all queries should call this first instead of GetList(), for example, if a UserBo has a getByUsername method,
        // then the method will call GetListQuery().Where(m => m.Username == username).
        // Calling GetList() then applying filters to it means that the database will return all records, then starts to filter
        // from those records, instead of filtering the query, then querying the database for the subsets. (BIG performance difference)
        public IQueryable<T> GetListQuery()
        {
            return dbSet;
        }

        public T GetById(int id)
        {
            return dbSet.Find(id);
        }

        public List<T> GetList()
        {
            return GetListQuery().ToList();
        }

        public void Add(T t)
        {
            dbSet.Add(t);
        }

        public void Remove(T t)
        {
            dbSet.Remove(t);
        }
    }
}
