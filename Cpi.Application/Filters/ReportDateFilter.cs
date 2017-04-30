using Cpi.Application.Models;
using System;
using System.Collections.Generic;

namespace Cpi.Application.Filters
{
    public class ReportDateFilter
    {
        public enum ReportDateIdEnums
        {
            Today = 1,
            Yesterday = 2,
            Past7Days = 3,
            Past30Days = 4,
            PastYear = 5,
            AllTimeOrSelectDateRange = 6
        }

        public static List<CpiSelectListItem> GetSelectList()
        {
            List<CpiSelectListItem> reportDates = new List<CpiSelectListItem>
            {
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Today, Name = "Today 今天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Yesterday, Name = "Yesterday 昨天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past7Days, Name = "Past 7 Days 近七天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past30Days, Name = "Past 30 Days 近三十天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.PastYear, Name = "Past Year 近一年" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.AllTimeOrSelectDateRange, Name = "All Time/Select Date Range 所有/自定期间" },
            };

            return reportDates;
        }

        public int? ReportDateId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
