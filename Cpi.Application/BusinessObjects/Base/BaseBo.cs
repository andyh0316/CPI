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
        // Calling GetList() then applying filters to it means that the database will return all records, then starts to filter
        // from those records, instead of filtering the query, then querying the database for the subsets. (BIG performance difference)
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

        public IQueryable<T> GetDateFilteredQuery(IQueryable<T> query, ReportDateFilter filter)
        {
            if (filter != null && filter.ReportDateId.HasValue)
            {
                if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Today)
                {
                    DateTime dateFrom = DateTime.Now.Date;
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Yesterday)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-1);
                    DateTime dateTo = DateTime.Now.Date;
                    query = query.Where(a => a.CreatedDate >= dateFrom && a.CreatedDate < dateTo);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past7Days)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-6);
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddDays(-29);
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.PastYear)
                {
                    DateTime dateFrom = DateTime.Now.Date.AddYears(-1);
                    query = query.Where(a => a.CreatedDate >= dateFrom);
                }
                else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.AllTimeOrSelectDateRange)
                {
                    if (filter.DateFrom.HasValue)
                    {
                        query = query.Where(a => a.CreatedDate >= filter.DateFrom.Value);
                    }

                    if (filter.DateTo.HasValue)
                    {
                        DateTime dateTo = filter.DateTo.Value.AddDays(1);
                        query = query.Where(a => a.CreatedDate < dateTo);
                    }
                }
            }

            return query;
        }

        public Tuple<DateTime, DateTime, bool> GetDateFilteredInfo(IQueryable<T> query, ReportDateFilter filter)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            bool splitByMonth = false;

            // we don't have today or yesterday here because if its one day we return null to indicate that the graphs don't need to show
            if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past7Days)
            {
                dateFrom = dateFrom.Date.AddDays(-6);
            }
            else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.Past30Days)
            {
                dateFrom = dateFrom.Date.AddDays(-29);
            }
            else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.PastYear)
            {
                DateTime dateLastYear = DateTime.Now.Date.AddYears(-1).AddMonths(1);
                dateFrom = new DateTime(dateLastYear.Year, dateLastYear.Month, 1);
                splitByMonth = true;
            }
            else if (filter.ReportDateId == (int)ReportDateFilter.ReportDateIdEnums.AllTimeOrSelectDateRange)
            {
                BaseDm earliestEntity = query.Where(a => a.CreatedDate.HasValue).OrderBy(a => a.CreatedDate.Value).FirstOrDefault();

                if (earliestEntity == null) // if theres no invoices with created date
                {
                    return null;
                }

                DateTime earliestEntityDate = earliestEntity.CreatedDate.Value; // this is guaranteed to have value because of the query and check above
                dateFrom = earliestEntityDate;

                if (filter.DateFrom.HasValue)
                {
                    if (filter.DateFrom.Value > dateFrom)
                    {
                        dateFrom = filter.DateFrom.Value;
                    }
                }

                if (filter.DateTo.HasValue)
                {
                    if (filter.DateTo.Value < dateTo)
                    {
                        dateTo = filter.DateTo.Value;
                    }
                }

                // if dateTo is more than a month away from dateFrom: do more things
                if (dateTo.AddMonths(-1) > dateFrom)
                {
                    splitByMonth = true;
                }
            }
            else
            {
                return null;
            }

            return new Tuple<DateTime, DateTime, bool>(dateFrom, dateTo, splitByMonth);
        }

        public void Commit()
        {
            CpiDbContext.SaveChanges();
        }
    }
}
