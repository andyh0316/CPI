using System;
using System.Collections.Generic;

namespace Cpi.Application.Filters
{
    public class ListFilter
    {
        public class BaseListFilter
        {
            public int Loads { get; set; }
            //public string SortColumn { get; set; }
            //public bool SortDesc { get; set; }
            public string SearchString { get; set; }

            public List<SortObject> SortObjects { get; set; }
        }

        public class SortObject
        {
            public string ColumnName { get; set; }
            public bool IsDescending { get; set; }
        }

        public class Call : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
                public int? StatusId { get; set; }
                public ReportDateFilter ReportDateFilter { get; set; }
            }
        }

        public class Invoice : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
                public int? StatusId { get; set; }
                public int? LocationId { get; set; }
                public ReportDateFilter ReportDateFilter { get; set; }
            }
        }

        public class Commodity : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
            }
        }

        public class User : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {

            }
        }

        public class Expense : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
                public ReportDateFilter ReportDateFilter { get; set; }
                public int? LocationId { get; set; }
            }
        }

        public class Finance : BaseListFilter
        {
            public AdvancedSearchClass AdvancedSearch { get; set; }
            public class AdvancedSearchClass
            {
            }
        }
    }
}
