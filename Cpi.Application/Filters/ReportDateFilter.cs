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
            Past30Days = 3,
            PastYear = 4,
            AllTime = 5,
            SelectDates = 6
        }

        public static List<CpiSelectListItem> GetSelectList()
        {
            List<CpiSelectListItem> reportDates = new List<CpiSelectListItem>
            {
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Today, Name = "Today 今天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Yesterday, Name = "Yesterday 昨天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past30Days, Name = "Past 30 Days 近三十天" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.PastYear, Name = "Past Year 近一年" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.AllTime, Name = "Beginning of Time 所有" },
                new CpiSelectListItem { Id = (int)ReportDateIdEnums.SelectDates, Name = "Select Your Dates 自定日期" }
            };

            return reportDates;
        }

        public int? ReportDateId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
