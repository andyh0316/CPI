using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cpi.ManageWeb.Models
{
    public class Pagination
    {
        public Pagination(int page, int total, int? take = 20)
        {
            Pages = Convert.ToInt32(Math.Ceiling((decimal)total / (decimal)take));
            Page = page;
            Skip = (page - 1) * take.Value;
            Take = take.Value;
            Total = total;
            From = (Total != 0) ? Skip + 1 : 0;
            To = (Total < Skip + Take) ? Total : Skip + Take;
        }

        public int Pages { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}