using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.Filters
{
    public class ClassFilter
    {
        public class Finance
        {
            public int? LocationId { get; set; }
            public ReportDateFilter ReportDateFilter { get; set; }
        }

        public class Performance
        {
            public bool AveragePerDay { get; set; }
            public ReportDateFilter ReportDateFilter { get; set; }
        }
    }
}
