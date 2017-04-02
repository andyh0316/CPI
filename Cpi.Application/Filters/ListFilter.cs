using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public class Invoice : BaseListFilter
        {
            //public AdvancedSearch AdvancedSearch { get; set; }
            public class AdvancedSearch
            {
                public DateTime? DateFrom { get; set; }
            }
        }
    }
}
