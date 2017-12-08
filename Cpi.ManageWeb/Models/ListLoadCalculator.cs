using System;
using static Cpi.Application.Filters.ListFilter;

namespace Cpi.ManageWeb.Models
{
    public class ListLoadCalculator
    {
        public int LoadedListItems { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string LoadGuid { get; set; } // everytime a list returns a new load, it will generate a random ID .. then angular will detect an ID change

        public ListLoadCalculator(BaseListFilter filter, int total, int? take = 50)
        {
            if (filter == null)
            {
                filter = new BaseListFilter();
            }

            int loads = filter.Loads;

            Total = total;
            Take = (filter.LoadMore) ? take.Value : take.Value * (loads + 1);
            Skip = (filter.LoadMore) ? loads * take.Value : 0;
            LoadedListItems = Skip + take.Value;
            LoadGuid = Guid.NewGuid().ToString();
        }
    }
}