using Cpi.Application.DataModels.LookUp;
using Cpi.Application.Helpers;
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
            PastYear = 5
        }

        public static List<CpiSelectListItem> GetSelectList()
        {
            List<CpiSelectListItem> reportDates = new List<CpiSelectListItem>();
            reportDates.Add(new CpiSelectListItem { Id = (int)ReportDateIdEnums.Today, Name = "Today ថ្ងៃនេះ" });
            reportDates.Add(new CpiSelectListItem { Id = (int)ReportDateIdEnums.Yesterday, Name = "Yesterday ម្សិលមិញ" });
            reportDates.Add(new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past7Days, Name = "Past 7 Days ៧ថ្ងៃមុន" });
            reportDates.Add(new CpiSelectListItem { Id = (int)ReportDateIdEnums.Past30Days, Name = "Past 30 Days ៣០ថ្ងៃមុន" });
            reportDates.Add(new CpiSelectListItem { Id = (int)ReportDateIdEnums.PastYear, Name = "Past Year ៣៦៥ថ្ងៃមុន" });

            //int roleId = UserHelper.GetRoleId();

            //if (roleId == (int)LookUpUserRoleDm.LookUpIds.Laozi || roleId == (int)LookUpUserRoleDm.LookUpIds.Admin)
            //{


            //}

            return reportDates;
        }

        public int? ReportDateId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
