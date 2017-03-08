using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Cpi.ManageWeb.Controllers.Base
{
    public class BaseController : Controller
    {
        public ContentResult JsonModel(object _object)
        {
            var model = new
            {
                Object = _object,
                //SessionTimeLeft = UserHelper.GetSessionTimeLeft()
            };

            return Content(JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { }));
        }

        // takes a query and applies sorting and pagination depending on the parameters passed
        //public IQueryable<T> GetPagedSortedQuery<T>(IQueryable<T> query, int skip, int take, string sortColumn, bool sortDesc)
        //{
        //    string sortDescString = (sortDesc) ? " descending" : "";
        //    query = query.OrderBy(sortColumn + sortDescString + ", Id");
        //    query = query.Skip(skip).Take(take);
        //    return query;
        //}
    }
}
