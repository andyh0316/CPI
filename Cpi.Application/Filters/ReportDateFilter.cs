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
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Today, Name = "Today" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Yesterday, Name = "Yesterday" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past7Days, Name = "Past 7 Days" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past30Days, Name = "Past 30 Days" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.PastYear, Name = "Past Year" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.AllTimeOrSelectDateRange, Name = "All Time/Select Date Range" },
            };

            return reportDates;
        }

        public int? ReportDateId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
