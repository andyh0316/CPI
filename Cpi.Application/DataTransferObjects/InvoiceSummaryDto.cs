using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataTransferObjects
{
    public class InvoiceSummaryDto
    {
        public List<Tuple<string, int>> Commodities { get; set; }
        public string Location { get; set; }
        //public int? Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
