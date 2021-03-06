﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Cpi.Application.DataModels.Base;
using Cpi.Application.Helpers;
using static Cpi.Application.Filters.ListFilter;

namespace Cpi.ManageWeb.Controllers.Base
{
    public class BaseController : Controller
    {
        public ContentResult JsonModel(object _object)
        {
            var model = new
            {
                Object = _object,
                SessionTimeLeft = UserHelper.GetSessionTimeLeft()
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
        public IQueryable<T> GetLoadedSortedQuery<T>(IQueryable<T> query, int skip, int take, List<SortObject> sortObjects)
        {
            string sortString = "";

            if (sortObjects != null)
            {
                foreach(SortObject sortObject in sortObjects)
                {
                    sortString = sortString + sortObject.ColumnName;
                    if (sortObject.IsDescending)
                    {
                        sortString = sortString + " " + "descending";
                    }
                    sortString = sortString + ",";
                }
            }

            sortString = sortString + "Id descending";

            query = query.OrderBy(sortString);
            query = query.Skip(skip).Take(take);
            return query;
        }
    }
}
