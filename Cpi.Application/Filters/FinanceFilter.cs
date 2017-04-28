using System;

namespace Cpi.Application.Filters
{
    public class FinanceFilter
    {
        public int? ReportDateId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
