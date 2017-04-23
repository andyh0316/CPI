using System;
using System.Collections.Generic;
using System.Linq;
namespace Cpi.Application.Filters
{
    public class ListFilter
    {
        public class BaseListFilter
        {
            public int Loads { get; set; }
            public string SortColumn { get; set; }
            public bool SortDesc { get; set; }
            public string SearchString { get; set; }
        }

        public class Call : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
                public DateTime? DateFrom { get; set; }
                public DateTime? DateTo { get; set; }
                public int? StatusId { get; set; }
                public bool TodayOnly { get; set; }
            }
        }

        public class User : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
            }
        }
    }
}
