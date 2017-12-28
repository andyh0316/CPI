using Cpi.Application.DatabaseContext;
using Cpi.Application.DataModels.Base;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Cpi.Application.Filters;
using System;

namespace Cpi.Application.BusinessObjects.Base
{
    public class BaseBo<T> where T : BaseDm
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

        public DbSet<T> DbSet
        {
            get
            {
                return CpiDbContext.Set<T>(); 
            }
        }

        // CAREFUL: all queries should call this first instead of GetList(), for example, if a UserBo has a getByUsername method,
        // then the method will call GetListQuery().Where(m => m.Username == username).
        // Calling GetList() then applying filters to it means that the database will return all listItems, then starts to filter
        // from those listItems, instead of filtering the query, then querying the database for the subsets. (BIG performance difference)
        public IQueryable<T> GetListQuery()
        {
            return DbSet;
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public List<T> GetList()
        {
            return GetListQuery().ToList();
        }

        public void Add(T t)
        {
            DbSet.Add(t);
        }

        public void Remove(T t)
        {
            DbSet.Remove(t);
        }

        public void RemoveRange(List<T> ts)
        {
            DbSet.RemoveRange(ts);
        }

        public IQueryable<T> GetListByIdsQuery(List<int> ids, bool ordered = false)
        {
            if (ids == null)
            {
                return Enumerable.Empty<T>().AsQueryable();
            }

            IQueryable<T> query = GetListQuery().Where(m => ids.Contains(m.Id));

            if (ordered) // order by the order of the ids
            {
                query = (from a in ids
                         join b in query
                         on a equals b.Id
                         select b).AsQueryable<T>();
            }

            return query;
        }

        public List<T> GetListByIds(List<int> ids, bool ordered = false)
        {
            if (ids == null)
            {
                return new List<T>();
            }

            return GetListByIdsQuery(ids, ordered).ToList();
        }

        public void Commit()
        {
            CpiDbContext.SaveChanges();
        }
    }
}
