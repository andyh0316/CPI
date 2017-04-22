using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Cpi.Application.DataModels.Base;
using Cpi.Application.Helpers;

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

        public ContentResult JsonModelState(ModelStateDictionary _ModelState)
        {
            Dictionary<string, string> ModelState = new Dictionary<string, string>();
            foreach (var key in _ModelState.Keys)
            {
                if (_ModelState[key].Errors.Count >= 1)
                {
                    // only add the first error: we expect the application to have one error per field at a time
                    ModelState.Add(key, _ModelState[key].Errors[0].ErrorMessage);
                }
            }

            var model = new
            {
                ModelState = ModelState
            };

            return Content(JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { }));
        }

        // takes a query and applies sorting and pagination depending on the parameters passed
        public IQueryable<T> GetLoadedSortedQuery<T>(IQueryable<T> query, int skip, int take, string sortColumn, bool sortDesc)
        {
            string sortDescString = (sortDesc) ? " descending" : "";
            query = query.OrderBy(sortColumn + sortDescString + ", Id");
            query = query.Skip(skip).Take(take);
            return query;
        }

        public void SetCreated(BaseDm baseDm)
        {
            baseDm.CreatedById = UserHelper.GetUserId();
            baseDm.CreatedDate = DateTime.Now;
        }

        public void SetModified(BaseDm baseDm)
        {
            baseDm.ModifiedById = UserHelper.GetUserId();
            baseDm.ModifiedDate = DateTime.Now;
        }
    }
}
